using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Vhilz.Avalonia.Theme;
using Xunit;

namespace Vhilz.Avalonia.Tests;

public class CaptionAccessibilityTests {
    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void KeyboardFocusAndLocalizedNameReachCaptionTemplate(bool light) {
        var template = DecorationTestTemplate.Build();
        var button = Assert.IsType<Button>(template.NameScope.Find("PART_CloseButton"));
        Assert.IsType<StackPanel>(button.Parent).Children.Remove(button);
        var window = new Window {
            Content = button,
            RequestedThemeVariant = light ? ThemeVariant.Light : ThemeVariant.Dark
        };
        window.Show();
        try {
            Assert.Equal("关闭", AutomationProperties.GetName(button));
            window.Resources[ResourceKeys.Text.Window.Close] = "Close window";
            Assert.Equal("Close window", AutomationProperties.GetName(button));
            var ring = Assert.Single(button.GetVisualDescendants().OfType<Border>(),
                border => border.Name == "PART_FocusRing");
            Assert.False(ring.IsVisible);
            button.Focus(NavigationMethod.Tab);
            Assert.True(button.IsFocused);
            Assert.True(ring.IsVisible);
            Assert.False(ring.IsHitTestVisible);
            // Headless 只检查焦点结构与状态，轮廓对比度仍需在 Demo 中实际观察。
        }
        finally {
            window.Close();
        }
    }
}
