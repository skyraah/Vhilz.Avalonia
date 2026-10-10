using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Input;
using Avalonia.Media;

namespace Vhilz.Avalonia.Demo.Components.N20;

public sealed class N20Sample : UserControl {
    public N20Sample() {
        var panel = new StackPanel { Spacing = 12, MaxWidth = 640, HorizontalAlignment = HorizontalAlignment.Stretch };
        var status = new TextBlock { Text = "尚未执行菜单命令" };
        MenuItem Action(string header) {
            var item = new MenuItem { Header = header };
            item.Click += (_, _) => status.Text = $"已执行：{header}";
            if (header == "新建文档") {
                item.InputGesture = new KeyGesture(Key.N, KeyModifiers.Control);
                item.Icon = new TextBlock { Text = "+", FontSize = 16 };
            }
            return item;
        }
        MenuItem[] Items() => new[] {
            Action("新建文档"),
            new MenuItem { Header = "显示工具栏", ToggleType = MenuItemToggleType.CheckBox, IsChecked = true },
            new MenuItem { Header = "排序", Items = { new MenuItem { Header = "按名称", ToggleType = MenuItemToggleType.Radio, GroupName = "sort", IsChecked = true }, new MenuItem { Header = "按修改时间", ToggleType = MenuItemToggleType.Radio, GroupName = "sort" } } },
            new MenuItem { Header = "-" },
            new MenuItem { Header = "当前不可用", IsEnabled = false },
            Action("关闭文档")
        };
        var menu = new Menu();
        var top = new MenuItem { Header = "文件" };
        foreach (var item in Items()) top.Items.Add(item);
        menu.Items.Add(top);
        panel.Children.Add(menu);
        var flyout = new MenuFlyout();
        foreach (var item in Items()) flyout.Items.Add(item);
        panel.Children.Add(new Button { Content = "下拉菜单", Flyout = flyout });
        var area = new Border { Padding = new Thickness(16), Child = new TextBlock { Text = "右键打开上下文菜单" } };
        area.ContextMenu = new ContextMenu();
        foreach (var item in Items()) area.ContextMenu.Items.Add(item);
        panel.Children.Add(area);
        panel.Children.Add(status);
        Content = panel;
    }
}
