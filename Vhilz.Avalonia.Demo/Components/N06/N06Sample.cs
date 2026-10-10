using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace Vhilz.Avalonia.Demo.Components.N06;

public sealed class N06Sample : UserControl {
    public N06Sample() {
        var panel = new StackPanel { Spacing = 16, MaxWidth = 720 };
        panel.Children.Add(new TextBlock { Text = "单选选项", FontSize = 20, FontWeight = FontWeight.SemiBold });
        panel.Children.Add(new TextBlock { Text = "通知方式" });
        var first = new RadioButton { Content = "所有通知", GroupName = "N06Notifications", IsChecked = true }; panel.Children.Add(first);
        panel.Children.Add(new RadioButton { Content = "仅重要通知", GroupName = "N06Notifications" }); panel.Children.Add(new RadioButton { Content = "停用通知（禁用）", GroupName = "N06Notifications", IsEnabled = false });
        panel.Children.Add(new TextBlock { Text = "同步频率 · 独立的第二组" });
        panel.Children.Add(new RadioButton { Content = "即时同步", GroupName = "N06Sync", IsChecked = true }); panel.Children.Add(new RadioButton { Content = new TextBlock { Text = "仅在使用无线网络时同步工作区中的所有项目和附件", TextWrapping = TextWrapping.Wrap, MaxWidth = 360 }, GroupName = "N06Sync" });
        var select = new Button { Content = "外部选择「所有通知」" }; select.Click += (_, _) => first.IsChecked = true; panel.Children.Add(select);
        Content = panel;
    }
}
