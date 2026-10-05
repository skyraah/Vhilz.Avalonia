using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Styling;
using Vhilz.Avalonia.Theme;
using Xunit;

namespace Vhilz.Avalonia.Tests;

// 统一搭建生产装饰模板，不为测试另写一套控件结构。
internal static class DecorationTestTemplate {
    internal static (WindowDrawnDecorationsContent Result, INameScope NameScope) Build() {
        var theme = new VhilzTheme();
        Assert.True(theme.TryGetResource(ResourceKeys.WindowDecorations.Theme, null, out var resource));
        var decorations = Assert.IsType<ControlTheme>(resource);
        var setter = Assert.Single(decorations.Setters.OfType<Setter>(),
            item => item.Property == WindowDrawnDecorations.TemplateProperty);
        var template = Assert.IsAssignableFrom<IWindowDrawnDecorationsTemplate>(setter.Value).Build();
        return (template.Result, template.NameScope);
    }
}
