using Avalonia.Controls;
using static Vhilz.Avalonia.Demo.Components.NativeSampleLayout;

namespace Vhilz.Avalonia.Demo.Components.E07;

public sealed class E07Sample : UserControl
{
    public E07Sample()
    {
        var status = Text("点击命令查看反馈；调节宽度观察溢出。");
        var bar = new CommandBar { Width = 500, DefaultLabelPosition = CommandBarDefaultLabelPosition.Right };
        foreach (var label in new[] { "新建", "保存", "复制", "共享", "删除" })
        {
            var command = new CommandBarButton { Label = label, Icon = Text("◇"), IsEnabled = label != "删除" };
            command.Click += (_, _) => status.Text = $"已执行：{label}";
            bar.PrimaryCommands.Add(command);
        }
        bar.PrimaryCommands.Add(new CommandBarToggleButton { Label = "收藏", Icon = Text("☆"), IsChecked = true });
        var secondary = new CommandBarButton { Label = "更多设置", Icon = Text("⚙") };
        secondary.Click += (_, _) => status.Text = "已打开更多设置";
        bar.SecondaryCommands.Add(secondary);
        var width = new Slider { Minimum = 180, Maximum = 600, Value = 500 };
        width.PropertyChanged += (_, e) => { if (e.Property == Slider.ValueProperty) bar.Width = width.Value; };
        Content = Column(Text("CommandBar · 自适应原生命令栏"), width, bar, status);
    }

}
