using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Input;
using Avalonia.Media;

namespace Vhilz.Avalonia.Demo.Components.N18;

public sealed class N18Sample : UserControl {
    public N18Sample() {
        var panel = new StackPanel { Spacing = 12, MaxWidth = 640, HorizontalAlignment = HorizontalAlignment.Stretch };
        foreach (var placement in new[] { PlacementMode.Top, PlacementMode.Bottom, PlacementMode.Left, PlacementMode.Right }) {
            var button = new Button { Content = $"{placement} 提示" };
            ToolTip.SetTip(button, "提示保留原生延时、位置与关闭行为");
            ToolTip.SetPlacement(button, placement);
            panel.Children.Add(button);
        }
        var longTip = new Button { Content = "长内容提示" };
        ToolTip.SetTip(longTip, new TextBlock { Text = "这段较长的提示用于观察多行内容、窗口边缘约束，以及在显示过程中切换主题的结果。", TextWrapping = TextWrapping.Wrap, MaxWidth = 240 });
        panel.Children.Add(longTip);
        var disabled = new Button { Content = "禁用触发器", IsEnabled = false };
        ToolTip.SetTip(disabled, "遵循 Avalonia 禁用宿主提示行为");
        panel.Children.Add(disabled);
        Content = panel;
    }
}
