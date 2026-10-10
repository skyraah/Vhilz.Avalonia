using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Input;
using Avalonia.Media;

namespace Vhilz.Avalonia.Demo.Components.N22;

public sealed class N22Sample : UserControl {
    public N22Sample() {
        var panel = new StackPanel { Spacing = 12, MaxWidth = 640, HorizontalAlignment = HorizontalAlignment.Stretch };
        panel.Children.Add(new TextBlock { Text = "日历输入型：可输入日期并校验" });
        panel.Children.Add(new CalendarDatePicker { PlaceholderText = "选择或输入日期", DisplayDateStart = DateTime.Today.AddMonths(-1), DisplayDateEnd = DateTime.Today.AddMonths(2) });
        panel.Children.Add(new CalendarDatePicker { SelectedDate = DateTime.Today });
        panel.Children.Add(new CalendarDatePicker { SelectedDate = DateTime.Today, IsEnabled = false });
        var invalid = new CalendarDatePicker { PlaceholderText = "校验提示" };
        DataValidationErrors.SetErrors(invalid, new[] { "请填写有效日期" });
        panel.Children.Add(invalid);
        panel.Children.Add(new TextBlock { Text = "原生字段选择型：选择后确认或取消" });
        panel.Children.Add(new DatePicker());
        panel.Children.Add(new DatePicker { SelectedDate = DateTimeOffset.Now });
        panel.Children.Add(new DatePicker { SelectedDate = DateTimeOffset.Now, IsEnabled = false });
        Content = panel;
    }
}
