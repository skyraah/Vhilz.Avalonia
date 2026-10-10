using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Vhilz.Avalonia.Demo.Components;

internal static class NativeSampleLayout
{
    internal static TextBlock Text(string value) => new() { Text = value, TextWrapping = TextWrapping.Wrap };

    internal static Button Action(string label, Action action)
    {
        var button = new Button { Content = label };
        button.Click += (_, _) => action();
        return button;
    }

    internal static StackPanel Column(params Control[] controls)
    {
        var panel = new StackPanel { Spacing = 12 };
        foreach (var control in controls) panel.Children.Add(control);
        return panel;
    }

    internal static ContentPage Page(string title, string text) => new()
    {
        Header = title, Content = new Border { Padding = new Thickness(20), Child = Text(text) }
    };
}
