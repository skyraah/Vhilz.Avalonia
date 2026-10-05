using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Vhilz.Avalonia.Theme;
using Vhilz.Avalonia.Theme.Controls;
using Xunit;

namespace Vhilz.Avalonia.Tests;

public class WindowPaletteTests {
    [AvaloniaFact]
    public void WindowPaletteTracksThemeActivationAndResourceOverrides() {
        var window = new VhilzWindow { Content = new TextBlock { Text = "正文" } };
        window.Show();
        try {
            var titleBar = Assert.Single(window.GetVisualDescendants().OfType<TitleBar>());
            foreach (var variant in new[] { ThemeVariant.Dark, ThemeVariant.Light }) {
                window.RequestedThemeVariant = variant;
                SetActive(window, true);
                AssertBrush(window, ResourceKeys.Window.BackgroundBrush, window.Background);
                AssertBrush(window, ResourceKeys.Window.ForegroundBrush, window.Foreground);
                AssertBrush(window, ResourceKeys.Window.BorderBrush, window.BorderBrush);
                AssertBrush(window, ResourceKeys.Window.TitleBar.BackgroundBrush, titleBar.Background);
                AssertBrush(window, ResourceKeys.Window.TitleBar.ForegroundBrush, titleBar.Foreground);
                AssertBrush(window, ResourceKeys.Window.TransparencyFallback.BackgroundBrush,
                    window.TransparencyBackgroundFallback);

                SetActive(window, false);
                AssertBrush(window, ResourceKeys.Window.Inactive.BorderBrush, window.BorderBrush);
                AssertBrush(window, ResourceKeys.Window.TitleBar.Inactive.ForegroundBrush, titleBar.Foreground);
                AssertBrush(window, ResourceKeys.Window.ForegroundBrush, ((TextBlock)window.Content!).Foreground);
                Assert.Equal(1, window.Opacity);
            }

            window.Resources[ResourceKeys.Window.BackgroundBrush] = Brushes.Navy;
            window.Resources[ResourceKeys.Window.TitleBar.BackgroundBrush] = Brushes.Maroon;
            window.Resources[ResourceKeys.Window.Inactive.BorderBrush] = Brushes.Olive;
            Assert.Same(Brushes.Navy, window.Background);
            Assert.Same(Brushes.Maroon, titleBar.Background);
            Assert.Same(Brushes.Olive, window.BorderBrush);

            // 应用显式设置仍高于主题，激活状态不能覆盖使用方的本地属性。
            window.BorderBrush = Brushes.Lime;
            window.TitleBarBackground = Brushes.Blue;
            SetActive(window, true);
            SetActive(window, false);
            Assert.Same(Brushes.Lime, window.BorderBrush);
            Assert.Same(Brushes.Blue, titleBar.Background);
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void DecorationPaletteTracksLogicalWindowIncludingFullscreenContent() {
        var template = DecorationTestTemplate.Build();
        var slots = template.Result;
        var underlay = slots.Underlay!;
        var overlay = slots.Overlay!;
        var popover = slots.FullscreenPopover!;
        slots.Underlay = slots.Overlay = slots.FullscreenPopover = null;
        // Headless 不保证生成平台装饰；将生产模板的三个区域挂到同一逻辑窗口验证资源与绑定。
        var panel = new StackPanel { Children = { underlay, overlay, popover } };
        var window = new VhilzWindow { Content = panel };
        window.Show();
        try {
            var buttons = panel.GetVisualDescendants().OfType<Button>().ToArray();
            var titles = panel.GetVisualDescendants().OfType<TextBlock>().ToArray();
            Assert.Equal(6, buttons.Length);
            Assert.Equal(2, titles.Length);
            foreach (var button in buttons)
                button.Transitions = null;

            foreach (var variant in new[] { ThemeVariant.Dark, ThemeVariant.Light }) {
                window.RequestedThemeVariant = variant;
                SetActive(window, true);
                foreach (var button in buttons)
                    AssertBrush(window, ResourceKeys.Brush.TextSecondary, button.Foreground);
                foreach (var title in titles)
                    AssertBrush(window, ResourceKeys.Window.TitleBar.ForegroundBrush, title.Foreground);

                SetActive(window, false);
                foreach (var button in buttons)
                    AssertBrush(window, ResourceKeys.Brush.TextMuted, button.Foreground);
                foreach (var title in titles)
                    AssertBrush(window, ResourceKeys.Window.TitleBar.Inactive.ForegroundBrush, title.Foreground);
                AssertBrush(window, ResourceKeys.Window.TitleBar.BackgroundBrush, ((DockPanel)popover).Background);
            }

            window.Resources[ResourceKeys.Brush.TextMuted] = Brushes.Orange;
            window.Resources[ResourceKeys.Window.TitleBar.Inactive.ForegroundBrush] = Brushes.Yellow;
            foreach (var button in buttons)
                Assert.Same(Brushes.Orange, button.Foreground);
            foreach (var title in titles)
                Assert.Same(Brushes.Yellow, title.Foreground);
        }
        finally {
            window.Close();
        }
    }

    // Headless 没有跨窗口的操作系统激活切换；经属性 setter 保留 Avalonia 的变更通知。
    private static void SetActive(Window window, bool active) =>
        typeof(WindowBase).GetProperty(nameof(WindowBase.IsActive))!
            .GetSetMethod(nonPublic: true)!.Invoke(window, new object[] { active });

    private static void AssertBrush(Control control, string key, IBrush? actual) {
        var expected = Assert.IsAssignableFrom<ISolidColorBrush>(control.FindResource(control.ActualThemeVariant, key));
        Assert.Equal(expected.Color, Assert.IsAssignableFrom<ISolidColorBrush>(actual).Color);
    }
}
