using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Vhilz.Avalonia.Demo.Catalog;
using Xunit;

namespace Vhilz.Avalonia.Demo.Tests;

public class DemoThemeTests {
    [AvaloniaFact]
    public void ThemeSelectorStartsFollowingSystemAndCanReturnAfterManualSelection() {
        var application = Application.Current!;
        var window = new ComponentCatalogWindow();
        try {
            window.Show();
            var selector = Assert.Single(window.GetVisualDescendants().OfType<ComboBox>(),
                comboBox => comboBox.Items.OfType<ComboBoxItem>().Any(item => Equals(item.Content, "跟随系统")));

            Assert.Equal(0, selector.SelectedIndex);
            Assert.Equal(ThemeVariant.Default, window.RequestedThemeVariant);
            Assert.Equal(application.ActualThemeVariant, window.ActualThemeVariant);

            selector.SelectedIndex = 1;
            Assert.Equal(ThemeVariant.Light, window.RequestedThemeVariant);
            Assert.Equal(ThemeVariant.Light, window.ActualThemeVariant);

            selector.SelectedIndex = 2;
            Assert.Equal(ThemeVariant.Dark, window.RequestedThemeVariant);
            Assert.Equal(ThemeVariant.Dark, window.ActualThemeVariant);

            selector.SelectedIndex = 0;
            Assert.Equal(ThemeVariant.Default, window.RequestedThemeVariant);
            Assert.Equal(application.ActualThemeVariant, window.ActualThemeVariant);
        }
        finally {
            window.Close();
        }
    }
}
