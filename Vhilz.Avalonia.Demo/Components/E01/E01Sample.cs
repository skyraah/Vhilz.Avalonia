using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Input;
using Avalonia.Media;

namespace Vhilz.Avalonia.Demo.Components.E01;

public sealed class E01Sample : UserControl {
    public E01Sample() {
        var panel = new StackPanel { Spacing = 12, MaxWidth = 640, HorizontalAlignment = HorizontalAlignment.Stretch };
        var tree = new TreeView { Height = 240 };
        var root = new TreeViewItem { Header = "工作空间", IsExpanded = true };
        var folder = new TreeViewItem { Header = "组件", IsExpanded = true };
        folder.Items.Add(new TreeViewItem { Header = "Button.axaml" });
        folder.Items.Add(new TreeViewItem { Header = "较长的组件主题文件名称.axaml" });
        folder.Items.Add(new TreeViewItem { Header = "不可用节点", IsEnabled = false });
        root.Items.Add(folder);
        root.Items.Add(new TreeViewItem { Header = "空节点" });
        tree.Items.Add(root);
        var status = new TextBlock { Text = "尚未选择节点" };
        tree.SelectionChanged += (_, _) => status.Text = $"当前节点：{(tree.SelectedItem as TreeViewItem)?.Header}";
        var add = new Button { Content = "添加节点" };
        var count = 0;
        add.Click += (_, _) => folder.Items.Add(new TreeViewItem { Header = $"新增节点 {++count}" });
        panel.Children.Add(tree);
        panel.Children.Add(status);
        panel.Children.Add(add);
        panel.Children.Add(new TreeView { IsEnabled = false, Items = { new TreeViewItem { Header = "禁用树", IsExpanded = true, Items = { new TreeViewItem { Header = "子节点" } } } } });
        Content = panel;
    }
}
