using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Input;
using Avalonia.Media;

namespace Vhilz.Avalonia.Demo.Components.N21;

public sealed class N21Sample : UserControl {
    public N21Sample() {
        var panel = new StackPanel { Spacing = 12, MaxWidth = 640, HorizontalAlignment = HorizontalAlignment.Stretch };
        var single = new Calendar { SelectedDate = DateTime.Today, SelectionMode = CalendarSelectionMode.SingleDate };
        single.BlackoutDates.Add(new CalendarDateRange(DateTime.Today.AddDays(2), DateTime.Today.AddDays(3)));
        panel.Children.Add(new TextBlock { Text = "单选：今天、其他月份与禁选日期" });
        panel.Children.Add(single);
        var status = new TextBlock { Text = $"已选日期：{single.SelectedDate:d}" };
        single.SelectedDatesChanged += (_, _) => status.Text = $"已选日期：{single.SelectedDate:d}";
        panel.Children.Add(status);
        panel.Children.Add(new TextBlock { Text = "连续区间：按住 Shift 选择" });
        panel.Children.Add(new Calendar { SelectionMode = CalendarSelectionMode.SingleRange, FirstDayOfWeek = DayOfWeek.Monday });
        panel.Children.Add(new Calendar { SelectionMode = CalendarSelectionMode.MultipleRange });
        panel.Children.Add(new Calendar { SelectedDate = DateTime.Today, IsEnabled = false });
        Content = panel;
    }
}
