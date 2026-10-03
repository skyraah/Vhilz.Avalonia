using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Vhilz.Avalonia.Theme.Controls;

namespace Vhilz.Avalonia.Demo;

public partial class MainWindow : VhilzWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnThemeSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox selector && Application.Current is { } application)
        {
            application.RequestedThemeVariant = selector.SelectedIndex switch
            {
                1 => ThemeVariant.Light,
                2 => ThemeVariant.Dark,
                _ => ThemeVariant.Default
            };
        }
    }
}
