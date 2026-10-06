using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace Vhilz.Avalonia.Theme.Controls;

// 命名属性承接模板资源，样式可直接覆盖某一控件的参数，无需新增全局资源键。
internal sealed partial class CaptionSurface {
    public static readonly StyledProperty<IBrush?> TargetBackgroundProperty =
        AvaloniaProperty.Register<CaptionSurface, IBrush?>(nameof(TargetBackground), null);
    public IBrush? TargetBackground {
        get => GetValue(TargetBackgroundProperty);
        set => SetValue(TargetBackgroundProperty, value);
    }

    public static readonly DirectProperty<CaptionSurface, IBrush?> NonSolidBackgroundProperty =
        AvaloniaProperty.RegisterDirect<CaptionSurface, IBrush?>(nameof(NonSolidBackground),
            surface => surface.NonSolidBackground);
    private IBrush? _nonSolidBackground;
    // 实色继续由 SurfaceColor 动画绘制；其他画笔交给模板中的标准 Border。
    public IBrush? NonSolidBackground => _nonSolidBackground;

    public static readonly StyledProperty<IBrush?> PressedBackgroundProperty =
        AvaloniaProperty.Register<CaptionSurface, IBrush?>(nameof(PressedBackground), null);
    public IBrush? PressedBackground {
        get => GetValue(PressedBackgroundProperty);
        set => SetValue(PressedBackgroundProperty, value);
    }

    public static readonly StyledProperty<IBrush?> ClosePressedBackgroundProperty =
        AvaloniaProperty.Register<CaptionSurface, IBrush?>(nameof(ClosePressedBackground), null);
    public IBrush? ClosePressedBackground {
        get => GetValue(ClosePressedBackgroundProperty);
        set => SetValue(ClosePressedBackgroundProperty, value);
    }

    public static readonly StyledProperty<bool> IsCloseProperty =
        AvaloniaProperty.Register<CaptionSurface, bool>(nameof(IsClose), false);
    public bool IsClose {
        get => GetValue(IsCloseProperty);
        set => SetValue(IsCloseProperty, value);
    }

    public static readonly StyledProperty<TimeSpan> DefaultDurationProperty =
        AvaloniaProperty.Register<CaptionSurface, TimeSpan>(nameof(DefaultDuration), TimeSpan.FromMilliseconds(120));
    public TimeSpan DefaultDuration {
        get => GetValue(DefaultDurationProperty);
        set => SetValue(DefaultDurationProperty, value);
    }

    public static readonly StyledProperty<TimeSpan> PressedDurationProperty =
        AvaloniaProperty.Register<CaptionSurface, TimeSpan>(nameof(PressedDuration), TimeSpan.FromMilliseconds(80));
    public TimeSpan PressedDuration {
        get => GetValue(PressedDurationProperty);
        set => SetValue(PressedDurationProperty, value);
    }

    public static readonly StyledProperty<TimeSpan> CloseResetDurationProperty =
        AvaloniaProperty.Register<CaptionSurface, TimeSpan>(nameof(CloseResetDuration), TimeSpan.FromMilliseconds(400));
    public TimeSpan CloseResetDuration {
        get => GetValue(CloseResetDurationProperty);
        set => SetValue(CloseResetDurationProperty, value);
    }

    public static readonly StyledProperty<Easing> DefaultEasingProperty =
        AvaloniaProperty.Register<CaptionSurface, Easing>(nameof(DefaultEasing), new SineEaseInOut());
    public Easing DefaultEasing {
        get => GetValue(DefaultEasingProperty);
        set => SetValue(DefaultEasingProperty, value);
    }

    public static readonly StyledProperty<Easing> CloseResetEasingProperty =
        AvaloniaProperty.Register<CaptionSurface, Easing>(nameof(CloseResetEasing), new CubicEaseOut());
    public Easing CloseResetEasing {
        get => GetValue(CloseResetEasingProperty);
        set => SetValue(CloseResetEasingProperty, value);
    }

    public static readonly StyledProperty<TimeSpan> TransformDurationProperty =
        AvaloniaProperty.Register<CaptionSurface, TimeSpan>(nameof(TransformDuration), TimeSpan.FromMilliseconds(160));
    public TimeSpan TransformDuration {
        get => GetValue(TransformDurationProperty);
        set => SetValue(TransformDurationProperty, value);
    }

    public static readonly StyledProperty<Easing> TransformEasingProperty =
        AvaloniaProperty.Register<CaptionSurface, Easing>(nameof(TransformEasing), new CubicEaseOut());
    public Easing TransformEasing {
        get => GetValue(TransformEasingProperty);
        set => SetValue(TransformEasingProperty, value);
    }

    private readonly ColorTransition _colorTransition = new() { Property = SurfaceColorProperty };
    private readonly TransformOperationsTransition _transformTransition = new() { Property = RenderTransformProperty };
    private readonly BrushTransition _foregroundTransition = new() { Property = TemplatedControl.ForegroundProperty };
    private IDisposable? _foregroundTransitionSetter;
    private Button? _foregroundTransitionOwner;

    public CaptionSurface() {
        Transitions = new Transitions { _colorTransition, _transformTransition };
        UpdateTransitionParameters();
    }

    private void AttachForegroundTransition() {
        var owner = IsClose ? TemplatedParent as Button : null;
        if (ReferenceEquals(_foregroundTransitionOwner, owner)) return;
        DetachForegroundTransition();
        // Transition 本身没有资源宿主；先让命名属性解析动态资源，再同步既有实例。
        if (owner is not null) {
            // 模板优先级低于应用的本地属性，保留使用方禁用或自定义动画的入口。
            // 只撤销本表面拥有的值，让角色退出和模板卸载还原下层配置。
            _foregroundTransitionSetter = owner.SetValue(TransitionsProperty,
                new Transitions { _foregroundTransition }, BindingPriority.Template);
            _foregroundTransitionOwner = owner;
        }
    }

    private void DetachForegroundTransition() {
        _foregroundTransitionSetter?.Dispose();
        _foregroundTransitionSetter = null;
        _foregroundTransitionOwner = null;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);
        // :pressed 的目标画笔先于 IsPressed 通知更新，按画笔身份判断进入按下态。
        if (change.Property == TargetBackgroundProperty || change.Property == PressedBackgroundProperty ||
            change.Property == ClosePressedBackgroundProperty || change.Property == IsCloseProperty ||
            change.Property == DefaultDurationProperty || change.Property == PressedDurationProperty ||
            change.Property == CloseResetDurationProperty || change.Property == DefaultEasingProperty ||
            change.Property == CloseResetEasingProperty || change.Property == TransformDurationProperty ||
            change.Property == TransformEasingProperty)
            UpdateTransitionParameters();
        if ((change.Property == IsCloseProperty || change.Property == TemplatedParentProperty) &&
            this.IsAttachedToVisualTree())
            AttachForegroundTransition();
        if (change.Property == TargetBackgroundProperty) {
            SetAndRaise(NonSolidBackgroundProperty, ref _nonSolidBackground,
                TargetBackground is ISolidColorBrush ? null : TargetBackground);
            SurfaceColor = (TargetBackground as ISolidColorBrush)?.Color ?? Colors.Transparent;
        }
        if (change.Property == RevealProximityDistanceProperty || change.Property == BoundsProperty ||
            change.Property == RevealBorderEnabledProperty)
            UpdateDecorationReveal();
    }

    private void UpdateTransitionParameters() {
        if (_colorTransition is null) return;
        var pressed = TargetBackground is not null &&
                      (ReferenceEquals(TargetBackground, PressedBackground) ||
                       ReferenceEquals(TargetBackground, ClosePressedBackground));
        var resetting = IsClose && TargetBackground is ISolidColorBrush { Color.A: 0 };
        _colorTransition.Duration = pressed ? PressedDuration : resetting ? CloseResetDuration : DefaultDuration;
        _colorTransition.Easing = resetting && !pressed ? CloseResetEasing : DefaultEasing;
        _transformTransition.Duration = TransformDuration;
        _transformTransition.Easing = TransformEasing;
        _foregroundTransition.Duration = DefaultDuration;
        _foregroundTransition.Easing = DefaultEasing;
    }
}
