using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Fluid.Avalonia.Acrylic;

namespace Vhilz.Avalonia.Theme.Controls;

// 仅适配 Avalonia 12 的独立装饰层；内容区仍使用 FAA 的材质和指针广播。
internal sealed partial class CaptionSurface : AcrylicSurface {
    private Interactive? _decorationRoot;
    private Point? _rootPointerPosition;
    private Point _revealPosition;
    private double _revealProximityIntensity;

    internal Point DecorationRevealPosition => _revealPosition;
    internal double DecorationRevealIntensity => RevealBorderEnabled ? _revealProximityIntensity : 0;

    static CaptionSurface() {
        AffectsRender<CaptionSurface>(RevealBorderEnabledProperty, RevealBorderColorProperty,
            RevealBorderWidthProperty, RevealBorderRadiusProperty, RevealBorderIntensityProperty);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) {
        base.OnAttachedToVisualTree(e);
        AttachForegroundTransition();
        // 装饰层与 Window 是视觉兄弟，FAA 的 GetTopLevel 无法识别该层。
        // 监听共同的视觉根，包含按钮外的接近范围，并保留已处理的指针事件。
        if (TopLevel.GetTopLevel(this) is not null || e.RootVisual is not Interactive root)
            return;
        _decorationRoot = root;
        root.AddHandler(PointerMovedEvent, OnRootPointerMoved, RoutingStrategies.Tunnel, handledEventsToo: true);
        root.AddHandler(PointerExitedEvent, OnRootPointerExited, RoutingStrategies.Direct, handledEventsToo: true);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) {
        DetachForegroundTransition();
        if (_decorationRoot is { } root) {
            root.RemoveHandler(PointerMovedEvent, OnRootPointerMoved);
            root.RemoveHandler(PointerExitedEvent, OnRootPointerExited);
            _decorationRoot = null;
        }

        _revealPosition = default;
        _rootPointerPosition = null;
        _revealProximityIntensity = 0;
        base.OnDetachedFromVisualTree(e);
    }

    public override void Render(DrawingContext context) {
        if (TopLevel.GetTopLevel(this) is not null) {
            base.Render(context);
            return;
        }

        // FAA 无背景快照时绘制白色占位层。窗口装饰不具备其采样条件，
        // 因此只绘制状态叠色与指针描边，常态保持透明，不伪造玻璃背景。
        var bounds = new Rect(Bounds.Size);
        var roundedBounds = new RoundedRect(bounds, CornerRadius);
        context.DrawRectangle(new SolidColorBrush(SurfaceColor), null, roundedBounds);
        if (DecorationRevealIntensity <= 0 || RevealBorderWidth <= 0 || RevealBorderRadius <= 0)
            return;

        var alpha = (byte)Math.Round(RevealBorderColor.A *
                                     Math.Clamp(RevealBorderIntensity * DecorationRevealIntensity, 0, 1));
        var color = Color.FromArgb(alpha, RevealBorderColor.R, RevealBorderColor.G, RevealBorderColor.B);
        var gradient = new RadialGradientBrush {
            Center = new RelativePoint(_revealPosition, RelativeUnit.Absolute),
            GradientOrigin = new RelativePoint(_revealPosition, RelativeUnit.Absolute),
            RadiusX = new RelativeScalar(RevealBorderRadius, RelativeUnit.Absolute),
            RadiusY = new RelativeScalar(RevealBorderRadius, RelativeUnit.Absolute),
            GradientStops = [new GradientStop(color, 0), new GradientStop(Colors.Transparent, 1)]
        };
        // 描边收进边界，避免被装饰层或圆角裁切。
        var inset = RevealBorderWidth / 2;
        var radius = new CornerRadius(
            Math.Max(0, CornerRadius.TopLeft - inset), Math.Max(0, CornerRadius.TopRight - inset),
            Math.Max(0, CornerRadius.BottomRight - inset), Math.Max(0, CornerRadius.BottomLeft - inset));
        context.DrawRectangle(null, new Pen(gradient, RevealBorderWidth),
            new RoundedRect(bounds.Deflate(inset), radius));
    }

    private void OnRootPointerMoved(object? sender, PointerEventArgs e) {
        _rootPointerPosition = e.GetPosition(_decorationRoot);
        UpdateDecorationReveal();
    }

    private void UpdateDecorationReveal() {
        if (_decorationRoot is null) return;
        var previousPosition = _revealPosition;
        var previousIntensity = _revealProximityIntensity;
        // 保存根坐标，控件重新布局后再转换；鼠标不动也能更新接近范围。
        var position = _rootPointerPosition is { } pointer ? _decorationRoot.TranslatePoint(pointer, this) : null;
        _revealPosition = position ?? default;
        _revealProximityIntensity = position is { } current
            ? CalculateProximity(current, Bounds.Size, RevealProximityDistance)
            : 0;
        // 无可见辉光时位置变化不影响像素；进入或离开有效范围才请求重绘。
        if (RevealBorderEnabled && (previousIntensity > 0 || _revealProximityIntensity > 0) &&
            (previousPosition != _revealPosition || previousIntensity != _revealProximityIntensity))
            InvalidateVisual();
    }

    private void OnRootPointerExited(object? sender, PointerEventArgs e) {
        _rootPointerPosition = null;
        UpdateDecorationReveal();
    }

    private static double CalculateProximity(Point position, Size size, double proximityDistance) {
        var dx = Math.Max(0, Math.Max(-position.X, position.X - size.Width));
        var dy = Math.Max(0, Math.Max(-position.Y, position.Y - size.Height));
        var distance = Math.Sqrt(dx * dx + dy * dy);
        return proximityDistance > 0 ? Math.Clamp(1 - distance / proximityDistance, 0, 1) : distance == 0 ? 1 : 0;
    }
}
