using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Input;
using Avalonia.Media;

namespace Vhilz.Avalonia.Demo.Components.N16;

public sealed class N16Sample : UserControl {
    public N16Sample() {
        var panel = new StackPanel { Spacing = 12, MaxWidth = 640, HorizontalAlignment = HorizontalAlignment.Stretch };
        var values = new[] { "Avalonia", "Avalonia UI", "C#", "Fluent", "Linear", "Linux", "macOS", "Windows" };
        var input = new AutoCompleteBox { ItemsSource = values, MinimumPrefixLength = 1, MinimumPopulateDelay = TimeSpan.Zero, PlaceholderText = "输入 a 或 l 查找建议", MaxDropDownHeight = 160 };
        var status = new TextBlock { Text = "尚未提交建议" };
        input.SelectionChanged += (_, _) => status.Text = $"当前建议：{input.SelectedItem ?? "无"}";
        panel.Children.Add(input);
        panel.Children.Add(status);
        panel.Children.Add(new AutoCompleteBox { ItemsSource = values, Text = "找不到的内容", PlaceholderText = "无结果仍保留输入" });
        panel.Children.Add(new AutoCompleteBox { ItemsSource = values, IsEnabled = false, Text = "Avalonia" });
        var invalid = new AutoCompleteBox { ItemsSource = values, PlaceholderText = "校验提示" };
        DataValidationErrors.SetErrors(invalid, new[] { "需要选择建议项" });
        panel.Children.Add(invalid);
        Content = panel;
    }
}
