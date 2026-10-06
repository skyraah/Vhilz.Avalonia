using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Platform;
using Avalonia.Animation;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Xunit;

namespace Vhilz.Avalonia.Demo.Tests;

public class DemoThemeTests {
    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void FixExamplesUseProductionThemesAndCloseRoleCanBeToggled(bool light) {
        var window = new MainWindow { RequestedThemeVariant = light ? ThemeVariant.Light : ThemeVariant.Dark };
        window.Show();
        try {
            var gradient = window.FindControl<Button>("GradientContractPreview")!;
            Assert.Contains(gradient.GetVisualDescendants().OfType<Border>(),
                border => border is { IsEffectivelyVisible: true, Background: IGradientBrush });
            var border = window.FindControl<Button>("BorderContractPreview")!;
            Assert.Contains(border.GetVisualDescendants().OfType<Border>(),
                item => item.IsEffectivelyVisible && ReferenceEquals(item.BorderBrush, border.BorderBrush));
            var alignment = window.FindControl<Button>("AlignmentContractPreview")!;
            Assert.Equal(HorizontalAlignment.Left, alignment.HorizontalContentAlignment);
            Assert.Equal(VerticalAlignment.Top, alignment.VerticalContentAlignment);
            var role = window.FindControl<Button>("RoleContractPreview")!;
            var toggle = window.FindControl<CheckBox>("ClosePreviewToggle")!;
            Assert.Single(role.Transitions!.OfType<BrushTransition>());
            toggle.IsChecked = false;
            Assert.Null(role.Transitions);
            toggle.IsChecked = true;
            Assert.Single(role.Transitions!.OfType<BrushTransition>());

            // 限制预览视口，确认长内容仍可滚动，而不是只检查外层容器存在。
            var scroll = window.FindControl<ScrollViewer>("InspectionScrollViewer")!;
            scroll.Height = 200;
            window.UpdateLayout();
            Assert.InRange(scroll.Viewport.Height, 1, 200);
            Assert.True(scroll.Extent.Height > scroll.Viewport.Height,
                $"Window={window.Bounds}, Scroll={scroll.Bounds}, Extent={scroll.Extent}, Viewport={scroll.Viewport}, Template={scroll.Template}");
        }
        finally {
            window.Close();
        }
    }

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
