using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Vhilz.Avalonia.Theme;
using Vhilz.Avalonia.Theme.Controls;
using Xunit;

namespace Vhilz.Avalonia.Tests;

public class ButtonTests {
    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void PointerAndDisabledStatesPreserveClickBehavior(bool light) {
        var button = new Button { Content = "按钮" };
        var window = new Window {
            Width = 400, Height = 200, Content = button,
            RequestedThemeVariant = light ? ThemeVariant.Light : ThemeVariant.Dark
        };
        var clicks = 0;
        button.Click += (_, _) => clicks++;
        window.Show();
        try {
            var presenter = Assert.Single(button.GetVisualDescendants().OfType<ContentPresenter>());
            Assert.Equal("PART_ContentPresenter", presenter.Name);
            Assert.True(presenter.RecognizesAccessKey);
            // 此处验证状态目标，渐变观感由 Demo 验收。
            presenter.Transitions = null;
            AssertBrush(button, ResourceKeys.Brush.Control, presenter.Background);
            var point = button.TranslatePoint(new Point(button.Bounds.Width / 2, button.Bounds.Height / 2), window)!
                .Value;
            window.MouseMove(point);
            Assert.True(button.IsPointerOver);
            AssertBrush(button, ResourceKeys.Brush.ControlHover, presenter.Background);
            window.MouseDown(point, MouseButton.Left);
            Assert.True(button.IsPressed);
            AssertBrush(button, ResourceKeys.Brush.ControlPressed, presenter.Background);
            window.MouseUp(point, MouseButton.Left);
            Assert.Equal(1, clicks);

            button.IsEnabled = false;
            AssertBrush(button, ResourceKeys.Brush.Control, presenter.Background);
            Assert.Equal(0.4, button.Opacity);
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            Assert.False(button.IsPressed);
            Assert.Equal(1, clicks);
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void PressAnimationCanChangeWhilePressed(bool light) {
        var button = new Button { Content = "按压反馈" };
        var window = new Window {
            Width = 400, Height = 200, Content = button,
            RequestedThemeVariant = light ? ThemeVariant.Light : ThemeVariant.Dark
        };
        window.Show();
        try {
            var presenter = Assert.Single(button.GetVisualDescendants().OfType<ContentPresenter>());
            presenter.Transitions = null;
            Assert.True(button.TryFindResource(ResourceKeys.Transform.PressFeedback,
                button.ActualThemeVariant, out var pressedTransform));
            Assert.Equal(ButtonPressAnimation.None, ButtonMotion.GetPressAnimation(button));
            var point = button.TranslatePoint(new Point(button.Bounds.Width / 2, button.Bounds.Height / 2), window)!
                .Value;
            window.MouseMove(point);
            window.MouseDown(point, MouseButton.Left);
            Assert.True(button.IsPressed);
            Assert.NotSame(pressedTransform, presenter.RenderTransform);
            ButtonMotion.SetPressAnimation(button, ButtonPressAnimation.Scale);
            Assert.Equal(ButtonPressAnimation.Scale, ButtonMotion.GetPressAnimation(button));
            Assert.Same(pressedTransform, presenter.RenderTransform);
            ButtonMotion.SetPressAnimation(button, ButtonPressAnimation.None);
            Assert.NotSame(pressedTransform, presenter.RenderTransform);
            ButtonMotion.SetPressAnimation(button, ButtonPressAnimation.Scale);
            Assert.Same(pressedTransform, presenter.RenderTransform);
            window.MouseUp(point, MouseButton.Left);
            Assert.NotSame(pressedTransform, presenter.RenderTransform);

            button.IsEnabled = false;
            window.MouseDown(point, MouseButton.Left);
            Assert.False(button.IsPressed);
            Assert.NotSame(pressedTransform, presenter.RenderTransform);
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void PressFeedbackKeepsButtonEdgesClickable() {
        var button = new Button { Content = "按压反馈" };
        ButtonMotion.SetPressAnimation(button, ButtonPressAnimation.Scale);
        var window = new Window { Width = 400, Height = 200, Content = button };
        var clicks = 0;
        button.Click += (_, _) => clicks++;
        window.Show();
        try {
            var presenter = Assert.Single(button.GetVisualDescendants().OfType<ContentPresenter>());
            presenter.Transitions = null;
            var point = button.TranslatePoint(new Point(1, button.Bounds.Height / 2), window)!.Value;
            window.MouseMove(point);
            window.MouseDown(point, MouseButton.Left);
            Assert.True(button.IsPressed);
            window.MouseUp(point, MouseButton.Left);
            Assert.Equal(1, clicks);
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void ContentTemplateAndOverridesSurviveThemeSwitch() {
        var contentTemplate = new FuncDataTemplate<string>((value, _) => new TextBlock { Text = $"内容：{value}" });
        var button = new Button {
            Content = "示例", ContentTemplate = contentTemplate,
            Padding = new Thickness(18, 9), HorizontalContentAlignment = HorizontalAlignment.Right
        };
        var window = new Window { Content = button, RequestedThemeVariant = ThemeVariant.Dark };
        window.Show();
        try {
            var presenter = Assert.Single(button.GetVisualDescendants().OfType<ContentPresenter>());
            presenter.Transitions = null;
            Assert.Same(contentTemplate, presenter.ContentTemplate);
            Assert.Equal("内容：示例", Assert.IsType<TextBlock>(presenter.Child).Text);
            Assert.Equal(button.Padding, presenter.Padding);
            Assert.Equal(HorizontalAlignment.Right, presenter.HorizontalContentAlignment);
            var darkColor = Assert.IsAssignableFrom<ISolidColorBrush>(presenter.Background).Color;
            window.RequestedThemeVariant = ThemeVariant.Light;
            AssertBrush(button, ResourceKeys.Brush.Control, presenter.Background);
            Assert.NotEqual(darkColor, Assert.IsAssignableFrom<ISolidColorBrush>(presenter.Background).Color);

            window.Resources[ResourceKeys.Radius.Small] = new CornerRadius(9);
            Assert.Equal(new CornerRadius(9), presenter.CornerRadius);
            button.Background = Brushes.Teal;
            window.RequestedThemeVariant = ThemeVariant.Dark;
            var point = button.TranslatePoint(new Point(button.Bounds.Width / 2, button.Bounds.Height / 2), window)!
                .Value;
            window.MouseMove(point);
            Assert.Same(Brushes.Teal, presenter.Background);
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void KeyboardFocusAndCommandRespectDisabledState() {
        var command = new RecordingCommand();
        var button = new Button { Content = "确认", Command = command, CommandParameter = "参数" };
        var window = new Window { Content = button };
        window.Show();
        try {
            var ring = Assert.Single(button.GetVisualDescendants().OfType<Border>(),
                border => border.Name == "PART_FocusRing");
            button.Focus(NavigationMethod.Tab);
            Assert.True(button.IsFocused);
            Assert.True(ring.IsVisible);
            Assert.False(ring.IsHitTestVisible);
            window.KeyPress(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null);
            Assert.True(button.IsPressed);
            window.KeyRelease(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null);
            Assert.False(button.IsPressed);
            Assert.Equal(1, command.Executions);
            Assert.Equal("参数", command.LastParameter);
            window.KeyPress(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null);
            window.KeyRelease(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null);
            Assert.Equal(2, command.Executions);

            command.Disable();
            Assert.False(button.IsEffectivelyEnabled);
            Assert.False(ring.IsVisible);
            window.KeyPress(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null);
            window.KeyRelease(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null);
            Assert.Equal(2, command.Executions);
        }
        finally {
            window.Close();
        }
    }

    private static void AssertBrush(Button button, string key, IBrush? actual) {
        Assert.True(button.TryFindResource(key, button.ActualThemeVariant, out var expected));
        Assert.Equal(Assert.IsAssignableFrom<ISolidColorBrush>(expected).Color,
            Assert.IsAssignableFrom<ISolidColorBrush>(actual).Color);
    }

    private sealed class RecordingCommand : ICommand {
        private bool _enabled = true;
        public int Executions { get; private set; }
        public object? LastParameter { get; private set; }
        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter) => _enabled;

        public void Execute(object? parameter) {
            Executions++;
            LastParameter = parameter;
        }

        public void Disable() {
            _enabled = false;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
