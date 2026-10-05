using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Vhilz.Avalonia.Demo;
using Xunit;

namespace Vhilz.Avalonia.Demo.Tests;

public class DemoThemeTests {
    [AvaloniaFact]
    public void ThemeSelectorStartsFollowingSystemAndCanReturnAfterManualSelection() {
        var application = Application.Current!;
        var previousVariant = application.RequestedThemeVariant;
        application.RequestedThemeVariant = ThemeVariant.Default;
        var window = new MainWindow();
        try {
            window.Show();
            var selector = Assert.Single(window.GetVisualDescendants().OfType<ComboBox>(),
                comboBox => comboBox.Items.OfType<ComboBoxItem>().Any(item => Equals(item.Content, "跟随系统")));

            Assert.Equal(0, selector.SelectedIndex);
            Assert.Equal(ThemeVariant.Default, application.RequestedThemeVariant);
            Assert.Equal(application.ActualThemeVariant, window.ActualThemeVariant);

            selector.SelectedIndex = 1;
            Assert.Equal(ThemeVariant.Light, application.RequestedThemeVariant);
            Assert.Equal(ThemeVariant.Light, window.ActualThemeVariant);

            selector.SelectedIndex = 2;
            Assert.Equal(ThemeVariant.Dark, application.RequestedThemeVariant);
            Assert.Equal(ThemeVariant.Dark, window.ActualThemeVariant);

            selector.SelectedIndex = 0;
            Assert.Equal(ThemeVariant.Default, application.RequestedThemeVariant);
            var systemVariant = application.PlatformSettings!.GetColorValues().ThemeVariant == PlatformThemeVariant.Dark
                ? ThemeVariant.Dark
                : ThemeVariant.Light;
            Assert.Equal(systemVariant, application.ActualThemeVariant);
            Assert.Equal(systemVariant, window.ActualThemeVariant);
        }
        finally {
            window.Close();
            application.RequestedThemeVariant = previousVariant;
        }
    }
}
