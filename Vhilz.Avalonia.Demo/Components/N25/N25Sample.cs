using Avalonia;
using Avalonia.Controls;
using static Vhilz.Avalonia.Demo.Components.NativeSampleLayout;

namespace Vhilz.Avalonia.Demo.Components.N25;

public sealed class N25Sample : UserControl
{
    public N25Sample()
    {
        var split = new SplitView { Height = 240, OpenPaneLength = 220, CompactPaneLength = 48, DisplayMode = SplitViewDisplayMode.CompactInline, IsPaneOpen = true,
            Pane = new Border { Padding = new Thickness(12), Child = Column(Text("工作区"), Text("项目"), Text("任务"), Text("设置")) },
            Content = new Border { Padding = new Thickness(20), Child = new ScrollViewer { Content = Text(string.Join("\n", Enumerable.Repeat("主内容 · 保留滚动和命中关系", 12))) } } };
        var modes = Column();
        foreach (var mode in Enum.GetValues<SplitViewDisplayMode>()) modes.Children.Add(Action(mode.ToString(), () => split.DisplayMode = mode));
        var positions = Column();
        foreach (var position in Enum.GetValues<SplitViewPanePlacement>()) positions.Children.Add(Action(position.ToString(), () => split.PanePlacement = position));
        Content = Column(Text("SplitView · 四种展示模式与四侧布局"), Action("展开 / 收起", () => split.IsPaneOpen = !split.IsPaneOpen), modes, positions, split);
    }

}
