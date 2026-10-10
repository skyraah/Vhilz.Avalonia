using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace Vhilz.Avalonia.Demo.Components.N04;

public sealed class N04Sample : UserControl {
    public N04Sample() {
        var panel = new StackPanel { Spacing = 16, MaxWidth = 720 };
        panel.Children.Add(new TextBlock { Text = "切换按钮", FontSize = 20, FontWeight = FontWeight.SemiBold });
        var row = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 }; var bold = new ToggleButton { Content = "B", IsChecked = true }; row.Children.Add(bold); row.Children.Add(new ToggleButton { Content = "斜体" }); row.Children.Add(new ToggleButton { Content = "禁用", IsEnabled = false }); row.Children.Add(new ToggleButton { Content = "禁用已选", IsChecked = true, IsEnabled = false }); panel.Children.Add(row);
        var state = new TextBlock(); void Update() => state.Text = $"加粗：{bold.IsChecked}"; bold.PropertyChanged += (_, e) => { if (e.Property == ToggleButton.IsCheckedProperty) Update(); }; Update(); panel.Children.Add(state);
        var three = new ToggleButton { Content = "三态切换", IsThreeState = true, IsChecked = null }; panel.Children.Add(three);
        var threeStatus = new TextBlock(); void ThreeUpdate() => threeStatus.Text = $"三态值：{three.IsChecked?.ToString() ?? "不确定"}"; three.PropertyChanged += (_, e) => { if (e.Property == ToggleButton.IsCheckedProperty) ThreeUpdate(); }; ThreeUpdate(); panel.Children.Add(threeStatus);
        Content = panel;
    }
}
