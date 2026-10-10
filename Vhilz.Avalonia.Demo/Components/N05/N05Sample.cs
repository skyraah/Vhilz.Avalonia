using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace Vhilz.Avalonia.Demo.Components.N05;

public sealed class N05Sample : UserControl {
    public N05Sample() {
        var panel = new StackPanel { Spacing = 16, MaxWidth = 720 };
        panel.Children.Add(new TextBlock { Text = "复选框", FontSize = 20, FontWeight = FontWeight.SemiBold });
        var check = new CheckBox { Content = "接受服务条款", IsChecked = true }; panel.Children.Add(check);
        var state = new TextBlock(); void Update() => state.Text = $"选中值：{check.IsChecked}"; check.PropertyChanged += (_, e) => { if (e.Property == ToggleButton.IsCheckedProperty) Update(); }; Update(); panel.Children.Add(state);
        panel.Children.Add(new CheckBox { Content = "三态：包括部分已选", IsThreeState = true, IsChecked = null });
        panel.Children.Add(new CheckBox { Content = new TextBlock { Text = "这是一个多行标签。点击文字也会切换复选框，长文本与勾选标记应保持清楚的对齐。", TextWrapping = TextWrapping.Wrap, MaxWidth = 360 } });
        panel.Children.Add(new CheckBox { Content = "禁用", IsEnabled = false }); panel.Children.Add(new CheckBox { Content = "禁用已选", IsEnabled = false, IsChecked = true });
        var external = new Button { Content = "切换第一项" }; external.Click += (_, _) => check.IsChecked = check.IsChecked != true; panel.Children.Add(external);
        Content = panel;
    }
}
