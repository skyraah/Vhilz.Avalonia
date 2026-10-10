using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Input;
using Avalonia.Media;

namespace Vhilz.Avalonia.Demo.Components.E02;

public sealed class E02Sample : UserControl {
    public E02Sample() {
        var panel = new StackPanel { Spacing = 12, MaxWidth = 640, HorizontalAlignment = HorizontalAlignment.Stretch };
        var input = new NumericUpDown { Minimum = 0, Maximum = 10, Increment = 0.5m, Value = 2.5m, FormatString = "0.0" };
        var status = new TextBlock { Text = "当前值：2.5" };
        input.ValueChanged += (_, _) => status.Text = $"当前值：{input.Value}";
        var reset = new Button { Content = "设置到最大值" };
        reset.Click += (_, _) => input.Value = input.Maximum;
        panel.Children.Add(input);
        panel.Children.Add(status);
        panel.Children.Add(reset);
        panel.Children.Add(new NumericUpDown { Value = null, PlaceholderText = "可空数值" });
        panel.Children.Add(new NumericUpDown { Value = 99.95m, Increment = 0.01m, FormatString = "F2", ButtonSpinnerLocation = Location.Left });
        panel.Children.Add(new NumericUpDown { Value = 3, IsReadOnly = true, AllowSpin = false });
        panel.Children.Add(new NumericUpDown { Value = 3, IsEnabled = false });
        var invalid = new NumericUpDown { Value = null };
        DataValidationErrors.SetErrors(invalid, new[] { "请输入有效数值" });
        panel.Children.Add(invalid);
        Content = panel;
    }
}
