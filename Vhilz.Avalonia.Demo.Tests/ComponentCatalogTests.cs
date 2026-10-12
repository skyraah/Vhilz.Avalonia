using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Vhilz.Avalonia.Demo.Catalog;
using Vhilz.Avalonia.Theme.Controls;
using Xunit;

namespace Vhilz.Avalonia.Demo.Tests;

public class ComponentCatalogTests {
    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void EveryRegisteredSampleLoadsWithProductionThemesAndCanBeReplaced(bool light) {
        var window = new ComponentCatalogWindow {
            RequestedThemeVariant = light ? ThemeVariant.Light : ThemeVariant.Dark
        };
        window.Show();
        try {
            var list = window.FindControl<TreeView>("ComponentList")!;
            var host = window.FindControl<ContentControl>("SampleHost")!;
            Assert.NotEmpty(list.Items);
            Control? previous = null;
            foreach (var group in list.Items.OfType<TreeViewItem>()) {
                group.IsExpanded = true;
                foreach (var item in group.Items.OfType<TreeViewItem>()) {
                    try {
                        list.SelectedItem = item;
                        window.UpdateLayout();
                    }
                    catch (Exception error) {
                        throw new InvalidOperationException($"样例加载失败：{group.Header} / {item.Header}", error);
                    }

                    var sample = Assert.IsAssignableFrom<UserControl>(host.Content);
                    Assert.NotEmpty(sample.GetVisualDescendants());
                    Assert.NotSame(previous, sample);
                    if (previous is not null)
                        Assert.Null(TopLevel.GetTopLevel(previous));
                    previous = sample;
                }
            }
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void WindowExampleFollowsThemeAndClosesWhenLeavingSample() {
        var owner = new ComponentCatalogWindow { RequestedThemeVariant = ThemeVariant.Dark };
        owner.Show();
        try {
            var host = owner.FindControl<ContentControl>("SampleHost")!;
            var open = Assert.Single(((Control)host.Content!).GetVisualDescendants().OfType<Button>());
            open.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            var preview = Assert.IsType<VhilzWindow>(Assert.Single(owner.OwnedWindows));
            preview.UpdateLayout();
            Assert.Contains(preview.GetVisualDescendants(), control => control is TitleBar);
            Assert.Equal(ThemeVariant.Dark, preview.ActualThemeVariant);
            owner.RequestedThemeVariant = ThemeVariant.Light;
            Assert.Equal(ThemeVariant.Light, preview.ActualThemeVariant);

            var minimize = preview.GetVisualDescendants().OfType<CheckBox>()
                .Single(option => Equals(option.Content, "显示最小化按钮"));
            minimize.IsChecked = false;
            Assert.False(preview.IsMinimizeButtonVisible);
            var title = Assert.Single(preview.GetVisualDescendants().OfType<TextBox>());
            title.Text = "修改后的标题";
            Assert.Equal("修改后的标题", preview.Title);

            open.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Assert.Single(owner.OwnedWindows);
            var list = owner.FindControl<TreeView>("ComponentList")!;
            list.SelectedItem = ((TreeViewItem)list.Items[0]!).Items[1];
            Assert.Empty(owner.OwnedWindows);
            Assert.False(preview.IsVisible);
        }
        finally {
            owner.Close();
        }
    }
}
