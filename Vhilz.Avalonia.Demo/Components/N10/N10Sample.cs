using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace Vhilz.Avalonia.Demo.Components.N10;

public sealed class N10Sample : UserControl {
    public N10Sample() {
        var panel = new StackPanel { Spacing = 16, MaxWidth = 720 };
        panel.Children.Add(new TextBlock { Text = "滑块", FontSize = 20, FontWeight = FontWeight.SemiBold });
        var slider = new Slider { Minimum = 0, Maximum = 100, Value = 35 }; panel.Children.Add(slider);
        var state = new TextBlock(); void Update() => state.Text = $"当前值：{slider.Value:0}"; slider.PropertyChanged += (_, e) => { if (e.Property == Slider.ValueProperty) Update(); }; Update(); panel.Children.Add(state);
        panel.Children.Add(new Slider { Minimum = 0, Maximum = 100, Value = 75, IsDirectionReversed = true });
        panel.Children.Add(new Slider { Minimum = 0, Maximum = 100, Value = 40, TickFrequency = 20, TickPlacement = TickPlacement.Outside, IsSnapToTickEnabled = true });
        panel.Children.Add(new Slider { Value = 0 }); panel.Children.Add(new Slider { Value = 100 }); panel.Children.Add(new Slider { Value = 40, IsEnabled = false });
        var vertical = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 24, Height = 160 }; vertical.Children.Add(new Slider { Value = 35, Orientation = Orientation.Vertical }); vertical.Children.Add(new Slider { Value = 75, Orientation = Orientation.Vertical, IsDirectionReversed = true }); panel.Children.Add(vertical);
        Content = panel;
    }
}
