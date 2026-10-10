using Avalonia;
using Avalonia.Controls;

namespace Vhilz.Avalonia.Theme.Controls;

public enum ButtonPressAnimation {
    None,
    Scale
}

public sealed class ButtonMotion : AvaloniaObject {
    private ButtonMotion() { }

    public static readonly AttachedProperty<ButtonPressAnimation> PressAnimationProperty =
        AvaloniaProperty.RegisterAttached<ButtonMotion, Button, ButtonPressAnimation>(
            "PressAnimation", ButtonPressAnimation.None);

    public static ButtonPressAnimation GetPressAnimation(Button button) =>
        button.GetValue(PressAnimationProperty);

    public static void SetPressAnimation(Button button, ButtonPressAnimation value) =>
        button.SetValue(PressAnimationProperty, value);
}
