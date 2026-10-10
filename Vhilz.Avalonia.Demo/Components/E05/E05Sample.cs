using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using static Vhilz.Avalonia.Demo.Components.NativeSampleLayout;

namespace Vhilz.Avalonia.Demo.Components.E05;

public sealed class E05Sample : UserControl
{
    public E05Sample()
    {
        var status = Text("操作结果会显示在此处");
        FlyoutBase Options()
        {
            var flyout = new MenuFlyout();
            foreach (var name in new[] { "导出 PDF", "导出图片", "复制链接" })
            {
                var item = new MenuItem { Header = name };
                item.Click += (_, _) => status.Text = $"已选择：{name}";
                flyout.Items.Add(item);
            }
            return flyout;
        }
        var split = new SplitButton { Content = "保存", Flyout = Options() };
        split.Click += (_, _) => status.Text = "已执行主操作：保存";
        var toggle = new ToggleSplitButton { Content = "订阅更新", Flyout = Options(), IsChecked = true };
        toggle.IsCheckedChanged += (_, _) => status.Text = toggle.IsChecked ? "已订阅更新" : "已取消订阅";
        Content = Column(Text("原生链接 / 下拉 / 分裂按钮"),
            new HyperlinkButton { Content = "打开 shadcn 官方文档", NavigateUri = new Uri("https://ui.shadcn.com/docs/components") },
            new DropDownButton { Content = "导出选项", Flyout = Options() }, split, toggle,
            new ToggleSplitButton { Content = "禁用订阅", IsEnabled = false, IsChecked = true, Flyout = Options() }, status);
    }

}
