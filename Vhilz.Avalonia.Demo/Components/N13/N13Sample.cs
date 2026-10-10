using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace Vhilz.Avalonia.Demo.Components.N13;

public sealed class N13Sample : UserControl {
    public N13Sample() {
        var panel = new StackPanel { Spacing = 16, MaxWidth = 720 };
        panel.Children.Add(new TextBlock { Text = "可调整分栏", FontSize = 20, FontWeight = FontWeight.SemiBold });
        panel.Children.Add(new TextBlock { Text = "拖动中间分隔线，或 Tab 聚焦后用方向键调整。两侧最小宽度为 100。" });
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("*,6,*"), Height = 160 };
        grid.ColumnDefinitions[0].MinWidth = 100; grid.ColumnDefinitions[2].MinWidth = 100;
        var left = new Border { Padding = new Thickness(16), Child = new TextBlock { Text = "项目列表" } }; grid.Children.Add(left);
        var split = new GridSplitter { ResizeDirection = GridResizeDirection.Columns, ResizeBehavior = GridResizeBehavior.PreviousAndNext, HorizontalAlignment = HorizontalAlignment.Stretch }; Grid.SetColumn(split, 1); grid.Children.Add(split);
        var right = new Border { Padding = new Thickness(16), Child = new TextBlock { Text = "项目详情" } }; Grid.SetColumn(right, 2); grid.Children.Add(right); panel.Children.Add(grid);
        var rows = new Grid { RowDefinitions = new RowDefinitions("*,6,*"), Height = 180 }; rows.RowDefinitions[0].MinHeight = 60; rows.RowDefinitions[2].MinHeight = 60;
        rows.Children.Add(new TextBlock { Text = "上方内容", Margin = new Thickness(16) });
        var rowSplit = new GridSplitter { ResizeDirection = GridResizeDirection.Rows, ResizeBehavior = GridResizeBehavior.PreviousAndNext, VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Stretch, ShowsPreview = true }; Grid.SetRow(rowSplit, 1); rows.Children.Add(rowSplit);
        var bottom = new TextBlock { Text = "下方内容 · 预览模式", Margin = new Thickness(16) }; Grid.SetRow(bottom, 2); rows.Children.Add(bottom); panel.Children.Add(rows);
        Content = panel;
    }
}
