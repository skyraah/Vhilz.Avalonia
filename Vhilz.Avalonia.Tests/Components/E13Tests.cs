using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.VisualTree;
using Xunit;

namespace Vhilz.Avalonia.Tests.Components;

public class E13Tests
{
    [AvaloniaFact]
    public void PagerNavigationButtonsKeepTheirCompactHitTarget()
    {
        var pager = new PipsPager { NumberOfPages = 5, SelectedPageIndex = 2, IsPreviousButtonVisible = true, IsNextButtonVisible = true };
        var window = new Window { Width = 400, Height = 200, Content = pager };
        window.Show();
        try
        {
            var buttons = pager.GetVisualDescendants().OfType<Button>().Where(x => x.Name is "PART_PreviousButton" or "PART_NextButton").ToArray();
            Assert.Equal(2, buttons.Length);
            Assert.All(buttons, button => Assert.Equal(24, button.Bounds.Height));
        }
        finally { window.Close(); }
    }
}

