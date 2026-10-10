using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Input;
using Avalonia.Media;

namespace Vhilz.Avalonia.Demo.Components.N17;

public sealed class N17Sample : UserControl {
    public N17Sample() {
        var panel = new StackPanel { Spacing = 12, MaxWidth = 640, HorizontalAlignment = HorizontalAlignment.Stretch };
        foreach (var placement in new[] { Dock.Top, Dock.Bottom, Dock.Left, Dock.Right }) {
            var tabs = new TabControl { TabStripPlacement = placement, MinHeight = 120, Items = {
                new TabItem { Header = "概览", Content = new TextBlock { Text = "概览内容" } },
                new TabItem { Header = "设置与较长标题", Content = new StackPanel { Children = { new TextBlock { Text = "较高的内容" }, new TextBox { PlaceholderText = "真实可编辑内容" } } } },
                new TabItem { Header = "不可用", IsEnabled = false, Content = "禁用内容" }
            } };
            panel.Children.Add(new TextBlock { Text = $"页签方向：{placement}" });
            panel.Children.Add(tabs);
        }
        panel.Children.Add(new TextBlock { Text = "独立 TabStrip" });
        panel.Children.Add(new TabStrip { ItemsSource = new[] { "源文件", "预览", "历史" }, SelectedIndex = 0 });
        Content = panel;
    }
}
