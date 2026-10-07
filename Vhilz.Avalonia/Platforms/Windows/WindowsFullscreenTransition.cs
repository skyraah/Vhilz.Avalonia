using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Threading;

namespace Vhilz.Avalonia.Theme.Platforms.Windows;

/// <summary>
/// 为 Win32 的即时全屏边界变更补充插值；全屏状态、目标矩形与恢复位置仍由 Avalonia 决定。
/// </summary>
internal sealed partial class WindowsFullscreenTransition : IFullscreenTransition {
    private const uint WindowPosChanging = 0x0046;
    private const uint DpiChanged = 0x02E0;
    private const uint SizeChanged = 0x0005;
    private const uint NoSize = 0x0001;
    private const uint NoMove = 0x0002;
    private const uint NoZOrder = 0x0004;
    private const uint NoActivate = 0x0010;
    private const uint GetAnimation = 0x0048;

    private readonly Window _window;
    private readonly nint _handle;
    private readonly DispatcherTimer _timer;
    private readonly Stopwatch _elapsed = new();
    private WindowState _previousState;
    private PixelRect _start;
    private PixelRect _target;
    private PixelRect? _interruptedBounds;
    private TimeSpan _duration;
    private IEasing _easing = new CubicEaseOut();
    private bool _applyingFrame;
    private bool _disposed;

    private WindowsFullscreenTransition(Window window, nint handle) {
        _window = window;
        _handle = handle;
        _previousState = window.WindowState;
        // 只驱动原生窗口位置；内容与命中布局由 Avalonia 的正常 Resize 通知更新。
        _timer = new DispatcherTimer(DispatcherPriority.Render) { Interval = TimeSpan.FromMilliseconds(16) };
        _timer.Tick += OnFrame;
        Win32Properties.AddWndProcHookCallback(window, OnWindowMessage);
    }

    internal static WindowsFullscreenTransition? TryAttach(Window window) {
        if (!OperatingSystem.IsWindows() || window.PlatformImpl is not IWin32OptionsTopLevelImpl)
            return null;
        var handle = window.TryGetPlatformHandle();
        return handle is { Handle: not 0, HandleDescriptor: "HWND" }
            ? new WindowsFullscreenTransition(window, handle.Handle)
            : null;
    }

    private nint OnWindowMessage(nint hwnd, uint message, nint wParam, nint lParam, ref bool handled) {
        if (_disposed || handled) return 0;
        if (_applyingFrame) {
            // 最大化进入全屏仍保留 WS_MAXIMIZE；我们追加的尺寸帧会收到 SIZE_MAXIMIZED。
            // 用 SIZE_RESTORED 走框架正常的全屏尺寸分支，避免误报为最大化而退出全屏。
            if (message != SizeChanged || wParam != 2 || _previousState != WindowState.FullScreen) return 0;
            handled = true;
            return SendMessage(hwnd, message, 0, lParam);

        }

        if (message == DpiChanged) {
            // 跨 DPI 时由系统重新计算坐标，结束本次插值，避免与系统推荐矩形竞争。
            Complete();
            return 0;
        }

        if (message != WindowPosChanging || !_window.IsVisible) return 0;

        var position = Marshal.PtrToStructure<NativeWindowPosition>(lParam);
        if ((position.Flags & NoSize) != 0 || !GetWindowRect(hwnd, out var nativeCurrent)) return 0;
        var current = nativeCurrent.ToPixelRect();
        var state = _window.WindowState;
        var crossing = (state == WindowState.FullScreen) != (_previousState == WindowState.FullScreen);
        var previousState = _previousState;
        _previousState = state;
        var requested = new PixelRect(
            (position.Flags & NoMove) != 0 ? current.X : position.X,
            (position.Flags & NoMove) != 0 ? current.Y : position.Y,
            position.Width, position.Height);

        if (crossing) {
            _timer.Stop();
            var interrupted = _interruptedBounds;
            _interruptedBounds = null;
            // 最小化及系统禁用动画时保持平台原行为；零时长也是应用的退出开关。
            if (previousState == WindowState.Minimized || state == WindowState.Minimized ||
                current.Width <= 0 || current.Height <= 0 || requested.Width <= 0 || requested.Height <= 0 ||
                current == requested || !TryReadMotion())
                return 0;
            current = interrupted ?? current;
            _start = current;
            _target = requested;
            // 第一次布局可能较慢；从首个实际动画帧计时，避免首帧直接跳到终点。
            _elapsed.Reset();
            _timer.Start();
        }
        else if (!_timer.IsEnabled) {
            return 0;
        }
        else if (requested != _target) {
            // 用户或平台的新尺寸请求优先，不让旧动画覆盖它。
            _timer.Stop();
            return 0;
        }

        // 截住平台的最终尺寸提交，随后按帧完成同一目标；不替换 WindowState 或平台保存的恢复位置。
        position.X = current.X;
        position.Y = current.Y;
        position.Width = current.Width;
        position.Height = current.Height;
        Marshal.StructureToPtr(position, lParam, false);
        return 0;
    }

    public void BeforeStateChange() {
        if (!_timer.IsEnabled || _applyingFrame) return;
        // 平台在进入全屏前读取恢复矩形；先提交上一目标，避免把中间帧保存为恢复尺寸。
        // 新动画仍从屏幕上的中间帧开始，反向操作保持连续。
        if (GetWindowRect(_handle, out var current)) _interruptedBounds = current.ToPixelRect();
        Complete();
    }

    public void BeforeHide() {
        _interruptedBounds = null;
        Complete();
    }

    private bool TryReadMotion() {
        var animation = new NativeAnimationInfo { Size = (uint)Marshal.SizeOf<NativeAnimationInfo>() };
        if (!SystemParametersInfo(GetAnimation, animation.Size, ref animation, 0) || animation.MinimizeAnimate == 0)
            return false;
        if (!_window.TryFindResource(ResourceKeys.Duration.Slow, out var duration) ||
            duration is not TimeSpan time || time <= TimeSpan.Zero)
            return false;
        _duration = time;
        if (_window.TryFindResource(ResourceKeys.Easing.Decelerate, out var easing) &&
            easing is IEasing value)
            _easing = value;
        return true;
    }

    private void OnFrame(object? sender, EventArgs e) {
        if (!_window.IsVisible) {
            _timer.Stop();
            return;
        }

        if (!_elapsed.IsRunning) _elapsed.Start();
        var progress = Math.Clamp(_elapsed.Elapsed.TotalMilliseconds / _duration.TotalMilliseconds, 0, 1);
        var eased = Math.Clamp(_easing.Ease(progress), 0, 1);
        var bounds = Interpolate(_start, _target, eased);
        if (!ApplyBounds(bounds) || progress >= 1)
            Complete();
    }

    private static PixelRect Interpolate(PixelRect from, PixelRect to, double progress) {
        progress = Math.Clamp(progress, 0, 1);
        return new PixelRect(Lerp(from.X, to.X), Lerp(from.Y, to.Y),
            Lerp(from.Width, to.Width), Lerp(from.Height, to.Height));
        int Lerp(int a, int b) => (int)Math.Round(a + ((double)b - a) * progress);
    }

    private bool ApplyBounds(PixelRect bounds) {
        _applyingFrame = true;
        try {
            return SetWindowPos(_handle, 0, bounds.X, bounds.Y, bounds.Width, bounds.Height, NoActivate | NoZOrder);
        }
        finally {
            _applyingFrame = false;
        }
    }

    private void Complete() {
        if (!_timer.IsEnabled) return;
        _timer.Stop();
        ApplyBounds(_target);
    }

    public void Dispose() {
        if (_disposed) return;
        _disposed = true;
        _timer.Stop();
        _timer.Tick -= OnFrame;
        Win32Properties.RemoveWndProcHookCallback(_window, OnWindowMessage);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeWindowPosition {
        public nint Handle, InsertAfter;
        public int X, Y, Width, Height;
        public uint Flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect {
        public int Left, Top, Right, Bottom;
        public readonly PixelRect ToPixelRect() => new(Left, Top, Right - Left, Bottom - Top);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeAnimationInfo {
        public uint Size;
        public int MinimizeAnimate;
    }

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetWindowRect(nint hwnd, out NativeRect rect);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetWindowPos(nint hwnd, nint insertAfter, int x, int y, int width, int height,
        uint flags);

    [LibraryImport("user32.dll", EntryPoint = "SendMessageW")]
    private static partial nint SendMessage(nint hwnd, uint message, nint wParam, nint lParam);

    [LibraryImport("user32.dll", EntryPoint = "SystemParametersInfoW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SystemParametersInfo(uint action, uint parameter, ref NativeAnimationInfo value,
        uint flags);
}
