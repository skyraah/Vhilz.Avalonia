using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.VisualTree;
using Xunit;

namespace Vhilz.Avalonia.Tests.Components;

public class N25Tests
{
    [AvaloniaFact]
    public void NativePanePartsRemainUsableForAllPlacementsAndDisplayModes()
    {
        var split = new SplitView { OpenPaneLength = 120, CompactPaneLength = 32,
            Pane = new TextBlock { Text = "导航" }, Content = new TextBlock { Text = "正文" } };
        var window = new Window { Width = 500, Height = 350, Content = split };
        window.Show();
        try
        {
            foreach (var placement in Enum.GetValues<SplitViewPanePlacement>())
            foreach (var mode in Enum.GetValues<SplitViewDisplayMode>())
            {
                split.PanePlacement = placement;
                split.DisplayMode = mode;
                split.IsPaneOpen = true;
                window.UpdateLayout();
                var pane = Assert.Single(split.GetVisualDescendants().OfType<Panel>(), x => x.Name == "PART_PaneRoot");
                Assert.True(placement is SplitViewPanePlacement.Left or SplitViewPanePlacement.Right ? pane.Bounds.Width == 120 : pane.Bounds.Height == 120);
                Assert.Contains(split.GetVisualDescendants(), x => x.Name == "PART_ContentPresenter");
                split.IsPaneOpen = false;
                window.UpdateLayout();
                var closedLength = mode is SplitViewDisplayMode.CompactInline or SplitViewDisplayMode.CompactOverlay ? 32 : 0;
                Assert.True(placement is SplitViewPanePlacement.Left or SplitViewPanePlacement.Right ? pane.Bounds.Width == closedLength : pane.Bounds.Height == closedLength);
            }
        }
        finally { window.Close(); }
    }
}
