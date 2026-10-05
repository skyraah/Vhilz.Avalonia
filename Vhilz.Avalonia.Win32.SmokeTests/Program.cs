using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Chrome;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using Avalonia.Styling;
using Avalonia.Threading;
using Vhilz.Avalonia.Theme;
using Vhilz.Avalonia.Theme.Controls;

namespace Vhilz.Avalonia.Win32.SmokeTests;

// 独立进程使用真实 Win32 后端；普通 Headless 测试无法覆盖窗口消息、恢复矩形及系统动画设置。
internal static class Program {
    internal static string ReportPath = "fullscreen-smoke.log";

    [STAThread]
    public static int Main(string[] args) {
        if (!OperatingSystem.IsWindows()) return 2;
        if (args.Length > 0) ReportPath = Path.GetFullPath(args[0]);
        return AppBuilder.Configure<SmokeApp>().UsePlatformDetect().StartWithClassicDesktopLifetime(args);
    }
}

internal sealed class SmokeApp : Application {
    private readonly List<string> _log = new();

    public override void Initialize() => Styles.Add(new VhilzTheme());

    public override void OnFrameworkInitializationCompleted() {
        var desktop = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;
        desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        Dispatcher.UIThread.Post(async () => {
            var exitCode = 0;
            try {
                foreach (var light in new[] { false, true }) {
                    await RunReveal(light);
                    if (Environment.GetEnvironmentVariable("VHILZ_REVEAL_ONLY") == "1") continue;
                    foreach (var scenario in new[] { "normal", "rapid", "maximized-normal", "maximized-maximized", "hide", "close" }) {
                        var baseline = await Run(scenario, false, light);
                        var animated = await Run(scenario, true, light);
                        Check(baseline == animated, $"{scenario}: 动画开关改变了最终状态或恢复矩形");
                        _log.Add($"PASS {(light ? "Light" : "Dark")} {scenario}: {animated}");
                    }
                }
            }
            catch (Exception e) {
                exitCode = 1;
                _log.Add($"FAIL {e}");
            }
            finally {
                File.WriteAllLines(Program.ReportPath, _log);
                desktop.Shutdown(exitCode);
            }
        });
        base.OnFrameworkInitializationCompleted();
    }

    private async Task<Snapshot?> Run(string scenario, bool animate, bool light) {
        var window = new VhilzWindow {
            Title = "Vhilz 全屏原生回归检查", Width = 720, Height = 480, ShowActivated = false,
            Position = new PixelPoint(180, 160), ExtendClientAreaToDecorationsHint = true,
            RequestedThemeVariant = light ? ThemeVariant.Light : ThemeVariant.Dark,
            Content = new TextBlock { Text = "自动检查完成后关闭", Margin = new Thickness(30) }
        };
        if (!animate) window.Resources[ResourceKeys.Duration.Slow] = TimeSpan.Zero;
        window.Show();
        var handle = window.TryGetPlatformHandle()!.Handle;
        var frames = new List<Snapshot>();
        var observer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
        observer.Tick += (_, _) => frames.Add(Read(window, handle));
        observer.Start();
        try {
            await Task.Delay(300);
            if (scenario.StartsWith("maximized", StringComparison.Ordinal)) {
                window.WindowState = WindowState.Maximized;
                await Task.Delay(450);
            }
            var start = Read(window, handle);
            var fullscreenBounds = window.Screens.ScreenFromWindow(window)!.Bounds;
            frames.Clear();
            window.WindowState = WindowState.FullScreen;
            if (scenario == "close") {
                await Task.Delay(65);
                observer.Stop();
                window.Close();
                await Task.Delay(350);
                return null;
            }
            if (scenario == "rapid") {
                await Task.Delay(65);
                window.WindowState = WindowState.Normal;
                await Task.Delay(40);
                window.WindowState = WindowState.FullScreen;
            }
            if (scenario == "hide") {
                await Task.Delay(65);
                window.Hide();
                await Task.Delay(300);
                window.Show();
            }
            await Task.Delay(600);
            var full = Read(window, handle);
            Check(full == new Snapshot(WindowState.FullScreen, fullscreenBounds), $"{scenario}: 未到达全屏目标，实际 {full}");
            if (scenario == "normal") CheckFrames(frames, start, full, animate, "进入");
            frames.Clear();
            window.WindowState = scenario == "maximized-maximized" ? WindowState.Maximized : WindowState.Normal;
            await Task.Delay(650);
            var restored = Read(window, handle);
            if (scenario == "normal") {
                Check(restored == start, "退出全屏未恢复初始状态和矩形");
                CheckFrames(frames, full, restored, animate, "退出");
            }
            return restored;
        }
        finally {
            observer.Stop();
            window.Close();
        }
    }

    private async Task RunReveal(bool light) {
        var content = new Border();
        var window = new VhilzWindow {
            Width = 720, Height = 480,
            ExtendClientAreaToDecorationsHint = true, Content = content,
            RequestedThemeVariant = light ? ThemeVariant.Light : ThemeVariant.Dark
        };
        window.Resources[ResourceKeys.Duration.Slow] = TimeSpan.Zero;
        Check(GetCursorPos(out var originalPointer), "无法保存系统指针位置");
        window.Show();
        try {
            window.WindowState = WindowState.FullScreen;
            await Task.Delay(300);
            var decorations = ((StyledElement)window.GetVisualParent()!).GetLogicalChildren()
                .OfType<WindowDrawnDecorations>().Single();
            var popover = decorations.Content!.FullscreenPopover!;
            var surface = popover;
            double Extent() => popover.Bounds.Height + popover.Margin.Top + popover.Margin.Bottom;
            var inset = window.GetVisualDescendants().OfType<Border>().Single(b => b.Name == VhilzWindow.PartFullscreenTitleBarInset);
            async Task Move(int x, int y, int delay) {
                // 使用真实指针，覆盖渲染后的命中重算及原生非客户区路径。
                var position = window.PointToScreen(new Point(x, y));
                Check(SetCursorPos(position.X, position.Y), "无法移动系统指针");
                await Task.Delay(delay);
            }
            for (var cycle = 0; cycle < 10; cycle++) {
                await Move(300, 200, 220);
                await Move(300, 0, 10);
                var previous = 0d;
                for (var step = 0; step < 20; step++) {
                    await Move(300 + step, Math.Min(step, 10), 10);
                    var top = content.TranslatePoint(default, window)!.Value.Y;
                    Check(popover.IsEffectivelyVisible && top >= previous,
                        $"慢速唤出第 {cycle} 次第 {step} 帧闪烁: visible={popover.IsEffectivelyVisible}, inset={previous}->{top}");
                    Check(Math.Abs(top - surface.TranslatePoint(default, window)!.Value.Y - popover.Bounds.Height - popover.Margin.Bottom) < 1,
                        "顶栏表面与正文未同步移动");
                    Check(inset.GetBaseValue(Control.HeightProperty).Value == Extent(), "动画基础目标被重置");
                    previous = top;
                }
                Check(Math.Abs(previous - Extent()) < 1, "展开后正文避让高度错误");
                Check(popover.Margin == new Thickness(2), "全屏顶栏外边距错误");
                var exit = popover.GetVisualDescendants().OfType<Button>()
                    .Single(button => button.Name == "PART_PopoverFullScreenButton");
                var close = popover.GetVisualDescendants().OfType<Button>()
                    .Single(button => button.Name == "PART_PopoverCloseButton");
                var from = exit.TranslatePoint(new Point(exit.Bounds.Width / 2, exit.Bounds.Height / 2), window)!.Value;
                var to = close.TranslatePoint(new Point(close.Bounds.Width / 2, close.Bounds.Height / 2), window)!.Value;
                // 真实指针跨越退出全屏与原生关闭按钮的命中边界，不点击按钮。
                for (var pass = 0; pass < 4; pass++) {
                    for (var step = 0; step <= 10; step++) {
                        var fraction = pass % 2 == 0 ? step / 10d : 1 - step / 10d;
                        await Move((int)Math.Round(from.X + (to.X - from.X) * fraction), (int)Math.Round(from.Y), 20);
                        var top = content.TranslatePoint(default, window)!.Value.Y;
                        Check(popover.IsEffectivelyVisible && Math.Abs(top - Extent()) < 1 &&
                              inset.GetBaseValue(Control.HeightProperty).Value == Extent(),
                            $"按钮间移动误收回: cycle={cycle}, pass={pass}, step={step}, inset={top}, target={inset.GetBaseValue(Control.HeightProperty)}");
                    }
                }
                var intermediate = 0;
                for (var step = 0; step < 20; step++) {
                    await Move(300 + step, 200, 10);
                    var top = content.TranslatePoint(default, window)!.Value.Y;
                    Check(top <= previous, "收回高度反向跳动");
                    if (top > 0 && top < Extent()) {
                        intermediate++;
                        Check(popover.IsEffectivelyVisible, "收回动画尚未完成就隐藏顶栏");
                        Check(Math.Abs(top - surface.TranslatePoint(default, window)!.Value.Y - popover.Bounds.Height - popover.Margin.Bottom) < 1,
                            "收回时顶栏与正文不同步");
                    }
                    previous = top;
                }
                Check(intermediate > 0, "收回未产生中间帧");
                Check(content.TranslatePoint(default, window)!.Value.Y == 0, "移出后残留空白");
                Check(!popover.IsEffectivelyVisible, "收回完成后顶栏仍可见");
                await Move(300, 0, 20);
                await Move(300, 200, 10);
                await Move(300, 0, 220);
                Check(popover.IsEffectivelyVisible && content.TranslatePoint(default, window)!.Value.Y == Extent(),
                    "快速移出再移入未完成展开");
            }
            _log.Add($"PASS {(light ? "Light" : "Dark")} reveal: 10 次慢速唤出、动画收回与快速反向，40 次按钮间穿越，四周间距 2");
        }
        finally {
            window.Close();
            SetCursorPos(originalPointer.X, originalPointer.Y);
        }
    }

    private void CheckFrames(List<Snapshot> frames, Snapshot from, Snapshot to, bool animate, string direction) {
        var intermediate = frames.Select(frame => frame.Bounds).Distinct()
            .Count(bounds => bounds != from.Bounds && bounds != to.Bounds);
        var settings = new AnimationInfo { Size = (uint)Marshal.SizeOf<AnimationInfo>() };
        var systemMotion = SystemParametersInfo(0x0048, settings.Size, ref settings, 0) && settings.MinimizeAnimate != 0;
        Check(animate && systemMotion ? intermediate >= 2 : intermediate == 0,
            $"{direction}全屏：动画设置与中间帧数量不符 ({intermediate})");
        Check(frames.All(frame => frame.State == to.State), $"{direction}全屏：动画帧改变了目标状态");
        _log.Add($"{direction}: animate={animate}, systemMotion={systemMotion}, intermediate={intermediate}");
    }

    private static void Check(bool condition, string message) {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static Snapshot Read(Window window, nint hwnd) {
        Check(GetWindowRect(hwnd, out var rect), "读取原生窗口矩形失败");
        return new Snapshot(window.WindowState, new PixelRect(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top));
    }

    private sealed record Snapshot(WindowState State, PixelRect Bounds);
    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect { public int Left, Top, Right, Bottom; }
    [StructLayout(LayoutKind.Sequential)]
    private struct AnimationInfo { public uint Size; public int MinimizeAnimate; }
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(nint hwnd, out NativeRect rect);
    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint { public int X, Y; }
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetCursorPos(out NativePoint point);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetCursorPos(int x, int y);
    [DllImport("user32.dll", EntryPoint = "SystemParametersInfoW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SystemParametersInfo(uint action, uint parameter, ref AnimationInfo value, uint flags);
}
