using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace Vhilz.Avalonia.Demo.Components.N07;

public sealed class N07Sample : UserControl {
    public N07Sample() {
        var panel = new StackPanel { Spacing = 16, MaxWidth = 720 };
        panel.Children.Add(new TextBlock { Text = "开关", FontSize = 20, FontWeight = FontWeight.SemiBold });
        var toggle = new ToggleSwitch { Content = "自动同步", OnContent = "已开启", OffContent = "已关闭" }; panel.Children.Add(toggle);
        var state = new TextBlock(); void Update() => state.Text = $"同步：{toggle.IsChecked}"; toggle.PropertyChanged += (_, e) => { if (e.Property == ToggleButton.IsCheckedProperty) Update(); }; Update(); panel.Children.Add(state);
        panel.Children.Add(new ToggleSwitch { IsChecked = true, OnContent = null, OffContent = null });
        panel.Children.Add(new ToggleSwitch { Content = new TextBlock { Text = "只在设备连接到无线网络时自动同步所有工作区项目", TextWrapping = TextWrapping.Wrap, MaxWidth = 360 }, IsChecked = true });
        panel.Children.Add(new ToggleSwitch { Content = "禁用关", IsEnabled = false, OffContent = "已关闭" }); panel.Children.Add(new ToggleSwitch { Content = "禁用开", IsChecked = true, IsEnabled = false, OnContent = "已开启" });
        Content = panel;
    }
}
