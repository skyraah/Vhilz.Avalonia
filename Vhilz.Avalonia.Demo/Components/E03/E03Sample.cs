using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Input;
using Avalonia.Media;

namespace Vhilz.Avalonia.Demo.Components.E03;

public sealed class E03Sample : UserControl {
    public E03Sample() {
        var panel = new StackPanel { Spacing = 12, MaxWidth = 640, HorizontalAlignment = HorizontalAlignment.Stretch };
        panel.Children.Add(new TextBlock { Text = "24 小时制：含秒字段" });
        panel.Children.Add(new TimePicker { ClockIdentifier = "24HourClock", UseSeconds = true });
        panel.Children.Add(new TimePicker { ClockIdentifier = "24HourClock", SelectedTime = new TimeSpan(14, 30, 0) });
        panel.Children.Add(new TextBlock { Text = "12 小时制" });
        panel.Children.Add(new TimePicker { ClockIdentifier = "12HourClock", SelectedTime = new TimeSpan(9, 15, 0) });
        panel.Children.Add(new TimePicker { SelectedTime = new TimeSpan(9, 15, 0), IsEnabled = false });
        Content = panel;
    }
}
