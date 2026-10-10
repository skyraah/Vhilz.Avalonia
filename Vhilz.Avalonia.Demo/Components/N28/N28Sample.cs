using Avalonia.Controls;
using Avalonia.Media;
using static Vhilz.Avalonia.Demo.Components.NativeSampleLayout;

namespace Vhilz.Avalonia.Demo.Components.N28;

public sealed class N28Sample : UserControl
{
    public N28Sample()
    {
        var direction = FlowDirection.LeftToRight;
        var region = Column(Text("方向继承 · 中文 / English / العربية"), new TextBox { Text = "مرحبا بالعالم · Hello world · 你好世界" });
        var menu = new MenuFlyout();
        menu.Items.Add(new MenuItem { Header = "项目", Items = { new MenuItem { Header = "打开" }, new MenuItem { Header = "归档" } } });
        region.Children.Add(new DropDownButton { Content = "导航菜单", Flyout = menu });
        region.Children.Add(new Border { FlowDirection = FlowDirection.LeftToRight, Child = Text("局部固定 LTR：INV-001 · 09:30") });
        var fixedRtl = Column(Text("固定 RTL：مرحبا"), new TextBox { Text = "مرحبا بالعالم" });
        fixedRtl.FlowDirection = FlowDirection.RightToLeft;
        Content = Column(Text("Direction · 运行时流向与局部覆盖"), Action("LTR / RTL", () => { direction = direction == FlowDirection.LeftToRight ? FlowDirection.RightToLeft : FlowDirection.LeftToRight; region.FlowDirection = direction; }), region, fixedRtl);
    }

}
