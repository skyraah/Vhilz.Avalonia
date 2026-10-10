using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using static Vhilz.Avalonia.Demo.Components.NativeSampleLayout;

namespace Vhilz.Avalonia.Demo.Components.N24;

public sealed class N24Sample : UserControl
{
    public N24Sample()
    {
        var status = Text("通知采用宿主窗口的官方管理器，最多 3 条。");
        WindowNotificationManager? manager = null;
        AdornerLayer? layer = null;
        var stack = Column(status);
        foreach (var type in new[] { NotificationType.Information, NotificationType.Success, NotificationType.Warning, NotificationType.Error })
        {
            stack.Children.Add(Action($"显示 {type}", () =>
            {
                if (manager is null)
                {
                    layer = AdornerLayer.GetAdornerLayer(this);
                    if (layer is null) return;
                    // 上游 host 构造函数没有公开卸载入口；示例显式管理自己的装饰层实例。
                    manager = new WindowNotificationManager { MaxItems = 3, Position = NotificationPosition.TopRight };
                    layer.Children.Add(manager);
                    AdornerLayer.SetAdornedElement(manager, layer);
                }
                manager.Show(new Notification($"{type} 通知", "本次更改已保存。较长的通知内容应保持自然换行，并可通过关闭按钮移除。", type, TimeSpan.FromSeconds(6)));
                status.Text = $"已请求 {type} 通知；6 秒后自动关闭。";
            }));
        }
        stack.Children.Add(Action("清除所有通知", () => manager?.CloseAll()));
        DetachedFromVisualTree += (_, _) =>
        {
            if (manager is null) return;
            manager.CloseAll();
            layer?.Children.Remove(manager);
            AdornerLayer.SetAdornedElement(manager, null);
            manager = null;
            layer = null;
        };
        foreach (var type in new[] { NotificationType.Success, NotificationType.Error })
        {
            var card = new NotificationCard { NotificationType = type, Content = Text($"{type}：原生通知卡的静态外观") };
            card.NotificationClosed += (_, _) => card.IsVisible = false;
            stack.Children.Add(card);
        }
        Content = stack;
    }

}
