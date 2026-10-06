using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Vhilz.Avalonia.Theme;
using Vhilz.Avalonia.Theme.Controls;
using Xunit;

namespace Vhilz.Avalonia.Tests;

public class CaptionMotionContractTests {
    [AvaloniaFact]
    public void CloseRoleCanBeRemovedAndReappliedWithoutLeavingTransitions() {
        var button = CreateCloseButton();
        var window = new Window { Content = button };
        window.Show();
        try {
            var surface = Assert.Single(button.GetVisualDescendants().OfType<CaptionSurface>());
            for (var cycle = 0; cycle < 3; cycle++) {
                Assert.True(surface.IsClose);
                Assert.Single(button.Transitions!.OfType<BrushTransition>());
                button.Classes.Remove("VhilzCaptionClose");
                Assert.False(surface.IsClose);
                Assert.Null(button.Transitions);
                button.Classes.Add("VhilzCaptionClose");
            }
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void ReplacingCaptionTemplateReturnsItsForegroundTransition() {
        var button = CreateCloseButton();
        var window = new Window { Content = button };
        window.Show();
        try {
            var surface = Assert.Single(button.GetVisualDescendants().OfType<CaptionSurface>());
            Assert.Single(button.Transitions!.OfType<BrushTransition>());
            button.Theme = new ControlTheme(typeof(Button));
            button.ClearValue(TemplatedControl.TemplateProperty);
            button.ApplyTemplate();
            Assert.False(surface.IsAttachedToVisualTree());
            Assert.Null(button.Transitions);
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void CaptionTemplatePreservesApplicationTransitions() {
        var theme = new VhilzTheme();
        Assert.True(theme.TryGetResource(ResourceKeys.CaptionButton.Theme, null, out var resource));
        var customTransition = new BrushTransition {
            Property = TemplatedControl.ForegroundProperty, Duration = TimeSpan.FromMilliseconds(650)
        };
        var customTransitions = new Transitions { customTransition };
        var button = new Button {
            Theme = Assert.IsType<ControlTheme>(resource), Transitions = customTransitions
        };
        button.Classes.Add("VhilzCaptionClose");
        var window = new Window { Content = button };
        window.Show();
        try {
            // 模板可以配置默认动画，但不能覆盖应用的本地属性或修改其过渡实例。
            window.Resources[ResourceKeys.CaptionButton.Duration.Color] = TimeSpan.FromMilliseconds(90);
            Assert.Same(customTransitions, button.Transitions);
            Assert.Equal(TimeSpan.FromMilliseconds(650), customTransition.Duration);
        }
        finally {
            window.Close();
        }
    }

    private static Button CreateCloseButton() {
        var theme = new VhilzTheme();
        Assert.True(theme.TryGetResource(ResourceKeys.CaptionButton.Theme, null, out var resource));
        var button = new Button { Theme = Assert.IsType<ControlTheme>(resource) };
        button.Classes.Add("VhilzCaptionClose");
        return button;
    }
}
