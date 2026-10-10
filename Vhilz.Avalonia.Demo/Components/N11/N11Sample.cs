using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace Vhilz.Avalonia.Demo.Components.N11;

public sealed class N11Sample : UserControl {
    public N11Sample() {
        var panel = new StackPanel { Spacing = 16, MaxWidth = 720 };
        panel.Children.Add(new TextBlock { Text = "进度", FontSize = 20, FontWeight = FontWeight.SemiBold });
        foreach (var value in new[] { 0d, 45d, 100d }) { panel.Children.Add(new TextBlock { Text = $"{value:0}%" }); panel.Children.Add(new ProgressBar { Value = value }); }
        var indeterminate = new ProgressBar { IsIndeterminate = true }; panel.Children.Add(indeterminate);
        var show = new CheckBox { Content = "显示不确定进度", IsChecked = true }; show.PropertyChanged += (_, e) => { if (e.Property == ToggleButton.IsCheckedProperty) indeterminate.IsVisible = show.IsChecked == true; }; panel.Children.Add(show);
        var progress = new ProgressBar { Value = 40, ShowProgressText = true, MinHeight = 24 }; panel.Children.Add(progress);
        var adjust = new Slider { Value = 40 }; adjust.PropertyChanged += (_, e) => { if (e.Property == Slider.ValueProperty) progress.Value = adjust.Value; }; panel.Children.Add(adjust);
        panel.Children.Add(new ProgressBar { Value = 60, IsEnabled = false });
        Content = panel;
    }
}
