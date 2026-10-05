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

public class TitlePresentationTests {
    [AvaloniaFact]
    public void TitleSettingsUpdateLiveWithoutHidingContentSlots() {
        var left = new Button { Content = "左侧", Width = 60 };
        var right = new Button { Content = "右侧", Width = 60 };
        var window = new VhilzWindow { Title = "窗口标题", Width = 500, LeftContent = left, RightContent = right };
        window.Show();
        try {
            var bar = Assert.Single(window.GetVisualDescendants().OfType<TitleBar>());
            var title = Assert.Single(bar.GetVisualDescendants().OfType<TextBlock>(),
                item => item.Name == "PART_WindowTitle");
            Assert.True(window.IsTitleVisible);
            Assert.True(bar.IsTitleVisible);
            Assert.True(title.IsVisible);
            Assert.Equal(TextAlignment.Left, window.TitleAlignment);
            Assert.Equal(TextAlignment.Left, title.TextAlignment);

            foreach (var variant in new[] { ThemeVariant.Dark, ThemeVariant.Light }) {
                window.RequestedThemeVariant = variant;
                window.TitleAlignment = TextAlignment.Center;
                window.UpdateLayout();
                Assert.Equal(TextAlignment.Center, bar.TitleAlignment);
                Assert.Equal(TextAlignment.Center, title.TextAlignment);
                Assert.True(title.Bounds.Width > title.TextLayout.Width);
                Assert.False(title.IsHitTestVisible);

                window.IsTitleVisible = false;
                Assert.False(bar.IsTitleVisible);
                Assert.False(title.IsVisible);
                Assert.True(bar.IsVisible);
                Assert.True(left.IsEffectivelyVisible);
                Assert.True(right.IsEffectivelyVisible);
                Assert.Equal("窗口标题", window.Title);

                window.Title = "更新后的标题";
                window.IsTitleVisible = true;
                Assert.True(title.IsVisible);
                Assert.Equal("更新后的标题", title.Text);
                window.TitleAlignment = TextAlignment.Left;
                Assert.Equal(TextAlignment.Left, title.TextAlignment);

                window.Title = string.Empty;
                window.IsTitleVisible = false;
                window.IsTitleVisible = true;
                Assert.False(title.IsVisible);
                window.Title = "窗口标题";
                Assert.True(title.IsVisible);
            }
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void DecorationAndFullscreenTitlesFollowWindowSettings() {
        var template = DecorationTestTemplate.Build();
        var slots = template.Result;
        var underlay = slots.Underlay!;
        var popover = slots.FullscreenPopover!;
        slots.Underlay = slots.FullscreenPopover = null;
        // Headless 不保证生成平台装饰；挂载生产模板，检查两处标题的窗口属性绑定。
        var content = new StackPanel { Children = { underlay, popover } };
        var window = new VhilzWindow { Content = content };
        window.Show();
        try {
            var titles = content.GetVisualDescendants().OfType<TextBlock>().ToArray();
            Assert.Equal(2, titles.Length);
            foreach (var title in titles)
                title.Text = "装饰标题";

            window.TitleAlignment = TextAlignment.Center;
            foreach (var title in titles) {
                Assert.True(title.IsVisible);
                Assert.Equal(TextAlignment.Center, title.TextAlignment);
            }
            window.IsTitleVisible = false;
            Assert.All(titles, title => Assert.False(title.IsVisible));
            window.TitleAlignment = TextAlignment.Left;
            window.IsTitleVisible = true;
            foreach (var title in titles) {
                Assert.True(title.IsVisible);
                Assert.Equal(TextAlignment.Left, title.TextAlignment);
                title.Text = string.Empty;
                Assert.False(title.IsVisible);
            }
        }
        finally {
            window.Close();
        }
    }
}
