using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace Vhilz.Avalonia.Theme.Controls;

// 全屏顶栏负责输入与避让动画；窗口保留上游 WindowState 和 Esc 处理契约。
internal sealed class FullscreenTitleBarController(VhilzWindow window) {
    private readonly VhilzWindow _window = window;
    private Border? _fullscreenTitleBarInset;
    private Control? _fullscreenPopover;
    private Control? _fullscreenPopoverLayer;
    private InputElement? _fullscreenPointerHost;
    private bool _fullscreenTitleBarRevealed;

    internal void SetInset(Border? inset) {
        if (_fullscreenTitleBarInset is not null)
            _fullscreenTitleBarInset.PropertyChanged -= OnInsetPropertyChanged;
        _fullscreenTitleBarInset = inset;
        if (inset is not null) inset.PropertyChanged += OnInsetPropertyChanged;
        UpdateFullscreenTitleBarInset(_fullscreenPopover);
    }

    private void OnInsetPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e) {
        // 动画帧仍须同步顶栏平移，但不再遍历装饰树或重算普通标题栏避让。
        if (e.Property == Control.BoundsProperty) UpdateFullscreenTitleBarInset(_fullscreenPopover);
    }

    internal void Reset() {
        _fullscreenTitleBarRevealed = false;
        UpdateFullscreenTitleBarInset(_fullscreenPopover);
    }

    internal void ClearPopover(Control host) {
        // 模板替换时旧部件可能晚于新部件卸载，不能清掉新登记。
        if (ReferenceEquals(_fullscreenPopover, host)) ObserveFullscreenPopover(null);
    }

    internal void ObserveFullscreenPopover(Control? popover) {
        if (ReferenceEquals(_fullscreenPopover, popover) &&
            ReferenceEquals(_fullscreenPopoverLayer, popover?.GetVisualParent())) return;
        if (_fullscreenPopover is not null)
            _fullscreenPopover.PropertyChanged -= OnFullscreenPopoverPropertyChanged;
        if (_fullscreenPointerHost is not null) {
            _fullscreenPointerHost.RemoveHandler(InputElement.PointerMovedEvent, OnFullscreenPointerMoved);
            _fullscreenPointerHost.RemoveHandler(InputElement.PointerExitedEvent, OnFullscreenPointerExited);
        }

        _fullscreenPopover = popover;
        // 上游会隐藏浮层的直接容器。收回动画需要暂时延长它的可见寿命；
        // 此处仍是 Avalonia 12 的兼容边界，升级时须用全屏测试核对，不能当作公开契约。
        _fullscreenPopoverLayer = popover?.GetVisualParent() as Control;
        _fullscreenPointerHost = popover?.GetVisualAncestors().LastOrDefault() as InputElement;
        _fullscreenTitleBarRevealed = popover is { IsEffectivelyVisible: true };
        if (_fullscreenPopover is not null)
            _fullscreenPopover.PropertyChanged += OnFullscreenPopoverPropertyChanged;
        if (_fullscreenPointerHost is not null) {
            // 装饰与窗口是兄弟；在共同宿主观察输入，保留按钮及正文自己的事件处理。
            // 框架的类处理器先执行，随后延长浮层寿命直到收回动画结束。
            _fullscreenPointerHost.AddHandler(InputElement.PointerMovedEvent, OnFullscreenPointerMoved,
                RoutingStrategies.Bubble, handledEventsToo: true);
            _fullscreenPointerHost.AddHandler(InputElement.PointerExitedEvent, OnFullscreenPointerExited,
                RoutingStrategies.Direct, handledEventsToo: true);
        }
    }

    private void OnFullscreenPointerMoved(object? sender, PointerEventArgs e) {
        if (_window.WindowState != WindowState.FullScreen || _fullscreenPopover is null) return;
        var position = e.GetPosition(_window);
        var extent = GetFullscreenTitleBarExtent(_fullscreenPopover);
        // 收回途中重新进入原顶栏范围即可反向，完全收起后仍沿用上游的 1 单位顶边触发区。
        var threshold = _fullscreenTitleBarRevealed || _fullscreenTitleBarInset?.Height > 0 ? extent : 1;
        _fullscreenTitleBarRevealed = position.X >= 0 && position.X <= _window.Bounds.Width &&
                                      position.Y >= 0 && position.Y <= threshold;
        UpdateFullscreenTitleBarInset(_fullscreenPopover);
    }

    private void OnFullscreenPointerExited(object? sender, PointerEventArgs e) {
        if (!ReferenceEquals(e.Source, _fullscreenPointerHost)) return;
        // Win32 在客户区按钮与原生关闭按钮之间切换时也会发出宿主退出事件，
        // 此时指针仍在顶栏内。沿用同一坐标边界判断，不能仅凭事件类型启动收回。
        OnFullscreenPointerMoved(sender, e);
    }

    private static double GetFullscreenTitleBarExtent(Control popover) =>
        popover.Bounds.Height + popover.Margin.Top + popover.Margin.Bottom;

    private void OnFullscreenPopoverPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e) {
        // 装饰是视觉兄弟，资源改变时可能晚于窗口布局；在其尺寸确定后立即同步避让目标。
        if (e.Property == Control.BoundsProperty || e.Property == Control.MarginProperty)
            UpdateFullscreenTitleBarInset(_fullscreenPopover);
    }

    private void UpdateFullscreenTitleBarInset(Control? popover) {
        if (_fullscreenTitleBarInset is null) return;

        var fullscreen = _window.WindowState == WindowState.FullScreen && _window.IsVisible && popover is not null;
        var extent = popover is null ? 0 : GetFullscreenTitleBarExtent(popover);
        if (fullscreen && (_fullscreenTitleBarRevealed || _fullscreenTitleBarInset.Bounds.Height > 0)) {
            var height = _fullscreenTitleBarRevealed ? extent : 0d;
            var targetHeight = _fullscreenTitleBarInset.GetBaseValue(Control.HeightProperty);
            // 布局会在动画每帧发生；仅目标改变时赋值，避免重设正在插值的属性。
            if (!targetHeight.HasValue || targetHeight.Value != height)
                // 此部件高度由窗口独占；必须写入基础值。SetCurrentValue 在动画期间
                // 会覆盖动画优先级的当前值而保留旧目标，完成时便跳回零并重新展开。
                _fullscreenTitleBarInset.SetValue(Control.HeightProperty, height);
        }
        else if (_fullscreenTitleBarInset.Height != 0 || _fullscreenTitleBarInset.IsAnimating(Control.HeightProperty)) {
            // 退出全屏、隐藏窗口或首帧尚未展开就移出时直接结算，避免待执行动画留下空隙。
            var transitions = _fullscreenTitleBarInset.Transitions;
            _fullscreenTitleBarInset.Transitions = null;
            _fullscreenTitleBarInset.SetValue(Control.HeightProperty, 0d);
            _fullscreenTitleBarInset.Transitions = transitions;
        }

        if (_fullscreenPopoverLayer is not null)
            _fullscreenPopoverLayer.SetCurrentValue(Control.IsVisibleProperty,
                fullscreen && (_fullscreenTitleBarRevealed || _fullscreenTitleBarInset.Height > 0));

        // 模板显式提供平移变换；以正文实际布局高度为准，动画中也保持边缘相接。
        if (popover?.RenderTransform is TranslateTransform translation) {
            translation.Y = fullscreen ? _fullscreenTitleBarInset.Bounds.Height - extent : -extent;
        }
    }
}
