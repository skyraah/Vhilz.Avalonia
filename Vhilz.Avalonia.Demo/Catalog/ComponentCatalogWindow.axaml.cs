using Avalonia.Controls;
using Avalonia.Styling;
using Vhilz.Avalonia.Theme.Controls;

namespace Vhilz.Avalonia.Demo.Catalog;

public partial class ComponentCatalogWindow : VhilzWindow {
    public ComponentCatalogWindow() {
        InitializeComponent();
        foreach (var group in ComponentCatalog.Groups) {
            var node = new TreeViewItem { Header = $"{group.Name} · {group.Entries.Count}" };
            foreach (var entry in group.Entries)
                node.Items.Add(new TreeViewItem { Header = entry.Name, Tag = entry });
            ComponentList.Items.Add(node);
        }
        var firstGroup = (TreeViewItem)ComponentList.Items[0]!;
        firstGroup.IsExpanded = true;
        ComponentList.SelectedItem = firstGroup.Items[0];
    }

    private void OnComponentChanged(object? sender, SelectionChangedEventArgs e) {
        if (ComponentList.SelectedItem is TreeViewItem { Tag: ComponentEntry entry }) {
            SampleHeading.Text = entry.Name;
            SampleHost.Content = entry.CreateSample();
            SampleScroll.Offset = default;
        }
    }

    private void OnThemeChanged(object? sender, SelectionChangedEventArgs e) {
        if (sender is ComboBox selector)
            RequestedThemeVariant = selector.SelectedIndex switch {
                1 => ThemeVariant.Light,
                2 => ThemeVariant.Dark,
                _ => ThemeVariant.Default
            };
    }
}
