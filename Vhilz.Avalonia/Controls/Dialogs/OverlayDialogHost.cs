namespace Vhilz.Avalonia.Theme.Controls;

/// <summary>
/// Vhilz 命名空间下的 Ursa 浮层宿主；注册、模态状态与布局直接继承基类。
/// </summary>
public class OverlayDialogHost : Ursa.Controls.OverlayDialogHost
{
    /// <inheritdoc />
    protected override Type StyleKeyOverride => typeof(OverlayDialogHost);
}
