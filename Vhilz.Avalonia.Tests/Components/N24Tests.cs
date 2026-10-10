using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Xunit;

namespace Vhilz.Avalonia.Tests.Components;

public class N24Tests
{
    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void CloseAllCompletesWithoutMutatingItsActiveEnumeration(bool light)
    {
        var manager = new WindowNotificationManager();
        var window = new Window { Width = 500, Height = 300, Content = manager,
            RequestedThemeVariant = light ? ThemeVariant.Light : ThemeVariant.Dark };
        window.Show();
        try
        {
            manager.Show(new Notification("第一条", "信息", expiration: TimeSpan.Zero));
            manager.Show(new Notification("第二条", "信息", expiration: TimeSpan.Zero));
            window.UpdateLayout();
            var cards = manager.GetVisualDescendants().OfType<NotificationCard>().ToArray();
            Assert.Equal(2, cards.Length);
            var clock = new TestAnimationClock(cards);
            manager.CloseAll();
            Assert.All(cards, card => Assert.True(card.IsClosing));
            clock.AdvanceBy(20);
            Assert.All(cards, card => Assert.True(card.IsClosed));
            Assert.Empty(manager.GetVisualDescendants().OfType<NotificationCard>());
        }
        finally { window.Close(); }
    }
}
