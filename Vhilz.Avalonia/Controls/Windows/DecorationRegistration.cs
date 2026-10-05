using Avalonia;
using Avalonia.Controls;

namespace Vhilz.Avalonia.Theme.Controls;

// 模板显式标注自有装饰部件；登记与卸载对称，不依赖上游宿主的私有结构。
internal sealed class DecorationRegistration : AvaloniaObject {
    // 继承属性让按钮组只绑定一次窗口激活状态，后代按钮共享同一状态源。
    public static readonly AttachedProperty<bool> IsInactiveProperty =
        AvaloniaProperty.RegisterAttached<DecorationRegistration, Control, bool>("IsInactive", inherits: true);
    public static bool GetIsInactive(Control host) => host.GetValue(IsInactiveProperty);
    public static void SetIsInactive(Control host, bool value) => host.SetValue(IsInactiveProperty, value);

    public static readonly AttachedProperty<VhilzWindow?> OwnerProperty =
        AvaloniaProperty.RegisterAttached<DecorationRegistration, Control, VhilzWindow?>("Owner");
    public static readonly AttachedProperty<bool> IsFullscreenProperty =
        AvaloniaProperty.RegisterAttached<DecorationRegistration, Control, bool>("IsFullscreen");

    public static VhilzWindow? GetOwner(Control host) => host.GetValue(OwnerProperty);
    public static void SetOwner(Control host, VhilzWindow? value) => host.SetValue(OwnerProperty, value);
    public static bool GetIsFullscreen(Control host) => host.GetValue(IsFullscreenProperty);
    public static void SetIsFullscreen(Control host, bool value) => host.SetValue(IsFullscreenProperty, value);

    static DecorationRegistration() {
        OwnerProperty.Changed.AddClassHandler<Control>((host, change) => {
            Register(host, change.GetOldValue<VhilzWindow?>(), false);
            host.AttachedToVisualTree -= OnAttached;
            host.DetachedFromVisualTree -= OnDetached;
            if (GetOwner(host) is null) return;
            host.AttachedToVisualTree += OnAttached;
            host.DetachedFromVisualTree += OnDetached;
            Register(host, GetOwner(host), true);
        });
    }

    private static void OnAttached(object? sender, VisualTreeAttachmentEventArgs e) {
        if (sender is Control host) Register(host, GetOwner(host), true);
    }

    private static void OnDetached(object? sender, VisualTreeAttachmentEventArgs e) {
        if (sender is Control host) Register(host, GetOwner(host), false);
    }

    private static void Register(Control host, VhilzWindow? owner, bool attached) {
        if (owner is null) return;
        if (GetIsFullscreen(host)) {
            if (attached) owner.SetFullscreenPopover(host);
            else owner.ClearFullscreenPopover(host);
        }
        else {
            if (attached) owner.SetCaptionButtons(host);
            else owner.ClearCaptionButtons(host);
        }
    }
}
