using Avalonia.Controls;
using Avalonia.Layout;
using static Vhilz.Avalonia.Demo.Components.NativeSampleLayout;

namespace Vhilz.Avalonia.Demo.Components.E13;

public sealed class E13Sample : UserControl
{
    public E13Sample()
    {
        var pager = new PipsPager { NumberOfPages = 8, SelectedPageIndex = 0, IsPreviousButtonVisible = true, IsNextButtonVisible = true };
        var status = Text("第 1 / 8 页");
        pager.SelectedIndexChanged += (_, _) => status.Text = $"第 {pager.SelectedPageIndex + 1} / {pager.NumberOfPages} 页";
        Content = Column(Text("PipsPager · 原生分页指示器"), pager, status,
            Action("横向 / 纵向", () => pager.Orientation = pager.Orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal),
            Action("8 / 30 页", () => { pager.NumberOfPages = pager.NumberOfPages == 8 ? 30 : 8; pager.SelectedPageIndex = 0; }),
            new PipsPager { NumberOfPages = 5, SelectedPageIndex = 2, IsEnabled = false });
    }

}
