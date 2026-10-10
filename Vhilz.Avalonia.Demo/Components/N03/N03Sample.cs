using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace Vhilz.Avalonia.Demo.Components.N03;

public sealed class N03Sample : UserControl {
    public N03Sample() {
        var panel = new StackPanel { Spacing = 16, MaxWidth = 720 };
        panel.Children.Add(new TextBlock { Text = "分隔线", FontSize = 20, FontWeight = FontWeight.SemiBold });
        panel.Children.Add(new TextBlock { Text = "正文中的水平分隔" }); panel.Children.Add(new Separator());
        var row = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 16, Height = 32 };
        row.Children.Add(new TextBlock { Text = "项目", VerticalAlignment = VerticalAlignment.Center });
        var vertical = new Separator(); vertical.Classes.Add("VhilzSeparatorVertical"); row.Children.Add(vertical);
        row.Children.Add(new TextBlock { Text = "设置", VerticalAlignment = VerticalAlignment.Center }); panel.Children.Add(row);
        panel.Children.Add(new TextBlock { Text = "打开右键菜单可观察菜单内的分隔线。" });
        panel.ContextFlyout = new MenuFlyout { Items = { new MenuItem { Header = "打开项目" }, new Separator(), new MenuItem { Header = "项目设置" } } };
        Content = panel;
    }
}
