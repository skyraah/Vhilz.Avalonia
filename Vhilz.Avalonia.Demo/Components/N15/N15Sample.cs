using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Input;
using Avalonia.Media;

namespace Vhilz.Avalonia.Demo.Components.N15;

public sealed class N15Sample : UserControl {
    public N15Sample() {
        var panel = new StackPanel { Spacing = 12, MaxWidth = 640, HorizontalAlignment = HorizontalAlignment.Stretch };
        var values = Enumerable.Range(1, 50).Select(i => $"选项 {i:00} · 长列表").ToArray();
        var select = new ComboBox { ItemsSource = values, SelectedIndex = 2, MaxDropDownHeight = 220 };
        var status = new TextBlock { Text = "当前选项：选项 03" };
        select.SelectionChanged += (_, _) => status.Text = $"当前选项：{select.SelectedItem}";
        panel.Children.Add(new TextBlock { Text = "纯选择：Enter 确认，Esc 取消" });
        panel.Children.Add(select);
        panel.Children.Add(status);
        panel.Children.Add(new ComboBox { ItemsSource = new[] { "短名称", "较长的名称，用于检查选择区、箭头与弹层的宽度分配" }, PlaceholderText = "请选择" });
        panel.Children.Add(new ComboBox { IsEditable = true, ItemsSource = values, Text = "可编辑原生 ComboBox" });
        panel.Children.Add(new ComboBox { ItemsSource = Array.Empty<string>(), PlaceholderText = "空列表" });
        panel.Children.Add(new ComboBox { IsEnabled = false, ItemsSource = values, SelectedIndex = 1 });
        var invalid = new ComboBox { ItemsSource = values, PlaceholderText = "校验提示" };
        DataValidationErrors.SetErrors(invalid, new[] { "请选择一个选项" });
        panel.Children.Add(invalid);
        Content = panel;
    }
}
