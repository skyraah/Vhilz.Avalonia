using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls.Presenters;

namespace Vhilz.Avalonia.Theme.Controls;

// Transition 没有模板宿主；由呈现器承接绑定，并为每个按钮持有独立的动画实例。
internal sealed class ButtonContentPresenter : ContentPresenter {
    public static readonly StyledProperty<bool> IsColorAnimationEnabledProperty =
        AvaloniaProperty.Register<ButtonContentPresenter, bool>(nameof(IsColorAnimationEnabled), true);
    public static readonly StyledProperty<TimeSpan> ColorAnimationDurationProperty =
        AvaloniaProperty.Register<ButtonContentPresenter, TimeSpan>(nameof(ColorAnimationDuration));
    public static readonly StyledProperty<Easing> ColorEasingProperty =
        AvaloniaProperty.Register<ButtonContentPresenter, Easing>(nameof(ColorEasing), new LinearEasing());
    public static readonly StyledProperty<TimeSpan> TransformDurationProperty =
        AvaloniaProperty.Register<ButtonContentPresenter, TimeSpan>(nameof(TransformDuration));
    public static readonly StyledProperty<Easing> TransformEasingProperty =
        AvaloniaProperty.Register<ButtonContentPresenter, Easing>(nameof(TransformEasing), new LinearEasing());

    private readonly BrushTransition _colorTransition = new() { Property = BackgroundProperty };
    private readonly TransformOperationsTransition _transformTransition = new() { Property = RenderTransformProperty };
    private readonly Transitions _transitions;

    public ButtonContentPresenter() {
        _transitions = [_colorTransition, _transformTransition];
        Transitions = _transitions;
    }

    public bool IsColorAnimationEnabled {
        get => GetValue(IsColorAnimationEnabledProperty);
        set => SetValue(IsColorAnimationEnabledProperty, value);
    }

    public TimeSpan ColorAnimationDuration {
        get => GetValue(ColorAnimationDurationProperty);
        set => SetValue(ColorAnimationDurationProperty, value);
    }

    public Easing ColorEasing {
        get => GetValue(ColorEasingProperty);
        set => SetValue(ColorEasingProperty, value);
    }

    public TimeSpan TransformDuration {
        get => GetValue(TransformDurationProperty);
        set => SetValue(TransformDurationProperty, value);
    }

    public Easing TransformEasing {
        get => GetValue(TransformEasingProperty);
        set => SetValue(TransformEasingProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);
        if (change.Property == ColorAnimationDurationProperty)
            _colorTransition.Duration = ColorAnimationDuration;
        else if (change.Property == ColorEasingProperty)
            _colorTransition.Easing = ColorEasing;
        else if (change.Property == TransformDurationProperty)
            _transformTransition.Duration = TransformDuration;
        else if (change.Property == TransformEasingProperty)
            _transformTransition.Easing = TransformEasing;
        else if (change.Property == IsColorAnimationEnabledProperty) {
            if (IsColorAnimationEnabled)
                _transitions.Add(_colorTransition);
            else
                _transitions.Remove(_colorTransition);
        }
    }
}
