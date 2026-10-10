using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Vhilz.Avalonia.Demo.Catalog;
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
            var list = window.FindControl<ListBox>("ComponentList")!;
            var host = window.FindControl<ContentControl>("SampleHost")!;
            Assert.NotEmpty(list.Items);
            Control? previous = null;
            for (var index = 0; index < list.ItemCount; index++) {
                try {
                    list.SelectedIndex = index;
                    window.UpdateLayout();
                }
                catch (Exception error) {
                    throw new InvalidOperationException($"样例加载失败：{list.Items[index]}", error);
                }

                var sample = Assert.IsAssignableFrom<UserControl>(host.Content);
                Assert.NotEmpty(sample.GetVisualDescendants());
                Assert.NotSame(previous, sample);
                if (previous is not null)
                    Assert.Null(TopLevel.GetTopLevel(previous));
                previous = sample;
            }
        }
        finally {
            window.Close();
        }
    }
}
