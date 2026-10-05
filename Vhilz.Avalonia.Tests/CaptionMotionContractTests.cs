using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Vhilz.Avalonia.Theme;
using Xunit;

namespace Vhilz.Avalonia.Tests;

public class CaptionMotionContractTests {
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
}
