using Avalonia;
using Avalonia.Controls;

namespace Vhilz.Avalonia.Theme.Controls;

public enum ButtonPressAnimation {
    None,
    Scale,
    Sink
}

public sealed class ButtonMotion : AvaloniaObject {
    private ButtonMotion() { }

    /// <summary>是否启用按下背景色，不影响缩放或下沉动效。</summary>
    public static readonly AttachedProperty<bool> IsPressStyleEnabledProperty =
        AvaloniaProperty.RegisterAttached<ButtonMotion, Button, bool>("IsPressStyleEnabled", true);

    /// <summary>是否启用悬停、按下及退出状态的背景颜色过渡。</summary>
    public static readonly AttachedProperty<bool> IsColorAnimationEnabledProperty =
        AvaloniaProperty.RegisterAttached<ButtonMotion, Button, bool>("IsColorAnimationEnabled", true);

    /// <summary>背景颜色过渡时长；基础主题动态使用 Fast Token，允许零，不接受负值。</summary>
    public static readonly AttachedProperty<TimeSpan> ColorAnimationDurationProperty =
        AvaloniaProperty.RegisterAttached<ButtonMotion, Button, TimeSpan>(
            "ColorAnimationDuration", validate: value => value >= TimeSpan.Zero);

    public static bool GetIsPressStyleEnabled(Button button) => button.GetValue(IsPressStyleEnabledProperty);

    public static void SetIsPressStyleEnabled(Button button, bool value) =>
        button.SetValue(IsPressStyleEnabledProperty, value);

    public static bool GetIsColorAnimationEnabled(Button button) => button.GetValue(IsColorAnimationEnabledProperty);

    public static void SetIsColorAnimationEnabled(Button button, bool value) =>
        button.SetValue(IsColorAnimationEnabledProperty, value);

    public static TimeSpan GetColorAnimationDuration(Button button) => button.GetValue(ColorAnimationDurationProperty);

    public static void SetColorAnimationDuration(Button button, TimeSpan value) =>
        button.SetValue(ColorAnimationDurationProperty, value);

    public static readonly AttachedProperty<ButtonPressAnimation> PressAnimationProperty =
        AvaloniaProperty.RegisterAttached<ButtonMotion, Button, ButtonPressAnimation>(
            "PressAnimation", ButtonPressAnimation.None);

    public static ButtonPressAnimation GetPressAnimation(Button button) =>
        button.GetValue(PressAnimationProperty);

    public static void SetPressAnimation(Button button, ButtonPressAnimation value) =>
        button.SetValue(PressAnimationProperty, value);
}
