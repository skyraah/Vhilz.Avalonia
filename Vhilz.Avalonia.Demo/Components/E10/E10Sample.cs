using Avalonia.Controls;
using static Vhilz.Avalonia.Demo.Components.NativeSampleLayout;

namespace Vhilz.Avalonia.Demo.Components.E10;

public sealed class E10Sample : UserControl
{
    public E10Sample()
    {
        var pages = new[] { Page("概览", "项目概览"), Page("活动记录和长标题", "最近活动"), Page("设置", "团队设置") };
        var tabbed = new TabbedPage { Height = 280, Pages = pages, SelectedIndex = 0 };
        var status = Text("当前页：概览");
        tabbed.CurrentPageChanged += (_, _) => status.Text = $"当前页：{tabbed.CurrentPage?.Header ?? "空"}";
        Content = Column(Text("TabbedPage · 页面集合和选中页"), tabbed, status,
            Action("空集合", () => tabbed.Pages = Array.Empty<Page>()),
            Action("恢复页面", () => { tabbed.Pages = pages; tabbed.SelectedIndex = 0; }));
    }

}
