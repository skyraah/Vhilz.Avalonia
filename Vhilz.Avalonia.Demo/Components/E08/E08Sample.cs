using Avalonia.Controls;
using static Vhilz.Avalonia.Demo.Components.NativeSampleLayout;

namespace Vhilz.Avalonia.Demo.Components.E08;

public sealed class E08Sample : UserControl
{
    public E08Sample()
    {
        var page = Page("概览", "ContentPage 内容由原生宿主呈现；标题由导航容器消费。\n\n" + string.Join("\n", Enumerable.Repeat("页面长内容与滚动", 15)));
        page.Height = 240;
        var body = page.Content;
        // 先释放旧逻辑父，再包装内容，避免旧 Content 的移除清掉 ScrollViewer 的逻辑父关系。
        page.Content = null;
        page.Content = new ScrollViewer { Content = body };
        Content = Column(Text("ContentPage · 标题和内容生命周期"), Text("独立 ContentPage 不自动制造导航标题栏。"), page,
            Action("更改标题", () => page.Header = page.Header?.ToString() == "概览" ? "项目详情" : "概览"),
            Action("显示 / 隐藏", () => page.IsVisible = !page.IsVisible));
    }

}
