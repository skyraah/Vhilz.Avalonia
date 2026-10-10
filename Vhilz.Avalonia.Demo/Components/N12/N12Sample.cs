using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace Vhilz.Avalonia.Demo.Components.N12;

public sealed class N12Sample : UserControl {
    public N12Sample() {
        var panel = new StackPanel { Spacing = 16, MaxWidth = 720 };
        panel.Children.Add(new TextBlock { Text = "展开与折叠", FontSize = 20, FontWeight = FontWeight.SemiBold });
        foreach (var direction in new[] { ExpandDirection.Down, ExpandDirection.Up, ExpandDirection.Left, ExpandDirection.Right })
            panel.Children.Add(new Expander { Header = $"展开方向 · {direction}", ExpandDirection = direction, Content = new TextBlock { Text = "内容使用原生布局；折叠后退出测量与命中。", TextWrapping = TextWrapping.Wrap } });
        var nested = new Expander { Header = "嵌套设置", IsExpanded = true, Content = new Expander { Header = "高级选项", Content = new TextBox { PlaceholderText = "展开后的输入框" } } }; panel.Children.Add(nested);
        var external = new CheckBox { Content = "展开嵌套设置", IsChecked = true }; external.PropertyChanged += (_, e) => { if (e.Property == ToggleButton.IsCheckedProperty) nested.IsExpanded = external.IsChecked == true; }; nested.PropertyChanged += (_, e) => { if (e.Property == Expander.IsExpandedProperty) external.IsChecked = nested.IsExpanded; }; panel.Children.Add(external);
        panel.Children.Add(new Expander { Header = "禁用已展开", IsExpanded = true, IsEnabled = false, Content = new TextBlock { Text = "仍然保留展开信息" } });
        Content = panel;
    }
}
