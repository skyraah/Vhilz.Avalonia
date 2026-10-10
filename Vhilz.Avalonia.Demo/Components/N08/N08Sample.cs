using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace Vhilz.Avalonia.Demo.Components.N08;

public sealed class N08Sample : UserControl {
    public N08Sample() {
        var panel = new StackPanel { Spacing = 16, MaxWidth = 720 };
        panel.Children.Add(new TextBlock { Text = "滚动区域", FontSize = 20, FontWeight = FontWeight.SemiBold });
        panel.Children.Add(new TextBlock { Text = "滚轮、拖动滚动条或触控滑动；横纵方向均可滚动。" });
        var rows = new StackPanel { Spacing = 12, Width = 900 };
        for (var i = 1; i <= 30; i++)
            rows.Children.Add(new TextBlock { Text = $"项目 {i:00} · 工作区资源 / Workspace resource", FontSize = 14 });
        panel.Children.Add(new ScrollViewer { Height = 260, Content = rows, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Auto });
        panel.Children.Add(new TextBlock { Text = "显式滚动条 · 拖动、轨道翻页与右键菜单" });
        panel.Children.Add(new ScrollBar { Orientation = Orientation.Horizontal, Minimum = 0, Maximum = 100, Value = 35, ViewportSize = 20 });
        panel.Children.Add(new ScrollBar { Orientation = Orientation.Horizontal, Minimum = 0, Maximum = 100, Value = 35, ViewportSize = 20, IsEnabled = false });
        Content = panel;
    }
}
