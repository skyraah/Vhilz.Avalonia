using Avalonia;
using Avalonia.Controls;
using static Vhilz.Avalonia.Demo.Components.NativeSampleLayout;

namespace Vhilz.Avalonia.Demo.Components.E11;

public sealed class E11Sample : UserControl
{
    public E11Sample()
    {
        var drawer = new DrawerPage { Height = 300, Header = "团队工作区", DrawerLength = 220, CompactDrawerLength = 48, IsOpen = true,
            Drawer = new Border { Padding = new Thickness(16), Child = Column(Text("导航"), new Button { Content = "概览" }, new Button { Content = "活动" }) },
            Content = Page("概览", "主页面内容") };
        var modes = Column();
        foreach (var mode in Enum.GetValues<SplitViewDisplayMode>()) modes.Children.Add(Action(mode.ToString(), () => drawer.DisplayMode = mode));
        Content = Column(Text("DrawerPage · 官方抽屉页面"), Action("打开 / 关闭", () => drawer.IsOpen = !drawer.IsOpen), modes, drawer);
    }

}
