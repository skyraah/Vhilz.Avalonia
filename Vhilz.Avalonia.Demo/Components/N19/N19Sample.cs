using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Input;
using Avalonia.Media;

namespace Vhilz.Avalonia.Demo.Components.N19;

public sealed class N19Sample : UserControl {
    public N19Sample() {
        var panel = new StackPanel { Spacing = 12, MaxWidth = 640, HorizontalAlignment = HorizontalAlignment.Stretch };
        var status = new TextBlock { Text = "尚未更新名称" };
        var name = new TextBox { Text = "工作空间" };
        var save = new Button { Content = "更新名称" };
        save.Click += (_, _) => status.Text = $"已更新：{name.Text}";
        var form = new StackPanel { Spacing = 8, Width = 260, Children = { new TextBlock { Text = "工作空间名称" }, name, save } };
        panel.Children.Add(new Button { Content = "编辑名称", Flyout = new Flyout { Content = form, Placement = PlacementMode.Bottom } });
        panel.Children.Add(status);
        foreach (var placement in new[] { PlacementMode.Top, PlacementMode.Left, PlacementMode.Right }) {
            panel.Children.Add(new Button { Content = $"{placement} 弹层", Flyout = new Flyout { Placement = placement, Content = new TextBox { Width = 240, PlaceholderText = "打开后切主题、Tab 或 Esc" } } });
        }
        var content = new StackPanel { Spacing = 6 };
        foreach (var i in Enumerable.Range(1, 35)) content.Children.Add(new TextBlock { Text = $"长内容条目 {i}" });
        panel.Children.Add(new Button { Content = "可滚动弹层", Flyout = new Flyout { Content = new ScrollViewer { MaxHeight = 220, Width = 240, Content = content } } });
        Content = panel;
    }
}
