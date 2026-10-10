using Avalonia.Controls;
using static Vhilz.Avalonia.Demo.Components.NativeSampleLayout;

namespace Vhilz.Avalonia.Demo.Components.E12;

public sealed class E12Sample : UserControl
{
    public E12Sample()
    {
        var pages = new[] { Page("第一步", "创建团队工作区"), Page("第二步", "邀请协作者并选择权限"), Page("第三步", "开始工作") };
        var carousel = new CarouselPage { Height = 220, Pages = pages, SelectedIndex = 0 };
        var status = Text("当前为第一步");
        carousel.CurrentPageChanged += (_, _) => status.Text = $"当前页：{carousel.CurrentPage?.Header ?? "空"}";
        Content = Column(Text("CarouselPage · 原生页面轮播"), carousel,
            Action("上一页", () => carousel.SelectedIndex = Math.Max(0, carousel.SelectedIndex - 1)),
            Action("下一页", () => carousel.SelectedIndex = Math.Min((carousel.Pages?.Count() ?? 1) - 1, carousel.SelectedIndex + 1)), status,
            Action("空集合", () => carousel.Pages = Array.Empty<Page>()),
            Action("单页", () => { carousel.Pages = pages.Take(1).ToArray(); carousel.SelectedIndex = 0; }),
            Action("恢复三页", () => { carousel.Pages = pages; carousel.SelectedIndex = 0; }));
    }

}
