using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Input;
using Avalonia.Media;

namespace Vhilz.Avalonia.Demo.Components.N14;

public sealed class N14Sample : UserControl {
    public N14Sample() {
        var panel = new StackPanel { Spacing = 12, MaxWidth = 640, HorizontalAlignment = HorizontalAlignment.Stretch };
        var single = new ListBox { Height = 180, ItemsSource = Enumerable.Range(1, 80).Select(i => $"项目 {i:00} · 原生虚拟化列表").ToArray(), SelectedIndex = 1 };
        var multiple = new ListBox { Height = 120, SelectionMode = SelectionMode.Multiple | SelectionMode.Toggle, ItemsSource = new[] { "设计", "工程", "产品", "这是一个需要横向滚动观察的很长的选项名称" }, SelectedIndex = 0 };
        var status = new TextBlock { Text = "当前单选：项目 02" };
        single.SelectionChanged += (_, _) => status.Text = $"当前单选：{single.SelectedItem}";
        panel.Children.Add(new TextBlock { Text = "单选、滚动与键盘导航" });
        panel.Children.Add(single);
        panel.Children.Add(status);
        panel.Children.Add(new TextBlock { Text = "多选：Ctrl / Shift 或直接切换" });
        panel.Children.Add(multiple);
        panel.Children.Add(new ListBox { Height = 64, ItemsSource = Array.Empty<string>() });
        panel.Children.Add(new ListBox { Height = 60, IsEnabled = false, ItemsSource = new[] { "禁用列表", "选中仍可辨认" }, SelectedIndex = 1 });
        panel.Children.Add(new ItemsControl { ItemsSource = new[] { "普通 ItemsControl", "无选择行为" } });
        Content = panel;
    }
}
