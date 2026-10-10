using Avalonia;
using Avalonia.Controls;
using static Vhilz.Avalonia.Demo.Components.NativeSampleLayout;

namespace Vhilz.Avalonia.Demo.Components.E09;

public sealed class E09Sample : UserControl
{
    public E09Sample()
    {
        var navigation = new NavigationPage { Height = 300 };
        var root = Page("工作区", "导航根页面");
        var status = Text("当前为根页");
        root.Content = new Border { Padding = new Thickness(20), Child = Column(Text("团队工作区"), Action("打开详情", () => _ = navigation.PushAsync(Page("项目详情", "这是第二页。使用导航栏返回按钮或下方返回操作。")))) };
        navigation.Content = root;
        var back = new Button { Content = "返回根页" };
        back.Click += async (_, _) => { await navigation.PopToRootAsync(); status.Text = "已回到根页"; };
        navigation.Pushed += (_, _) => status.Text = $"导航深度：{navigation.StackDepth}";
        navigation.Popped += (_, _) => status.Text = $"导航深度：{navigation.StackDepth}";
        Content = Column(Text("NavigationPage · 原生导航栈"), navigation, back, status);
    }

}
