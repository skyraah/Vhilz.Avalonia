using Avalonia.Controls;
using Avalonia.Styling;
using Vhilz.Avalonia.Theme.Controls;

namespace Vhilz.Avalonia.Demo.Catalog;

public partial class ComponentCatalogWindow : VhilzWindow {
    public ComponentCatalogWindow() {
        InitializeComponent();
        ComponentList.ItemsSource = ComponentCatalog.Entries;
        ComponentList.SelectedIndex = 0;
    }

    private void OnComponentChanged(object? sender, SelectionChangedEventArgs e) {
        if (ComponentList.SelectedItem is ComponentEntry entry)
            SampleHost.Content = entry.CreateSample();
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
