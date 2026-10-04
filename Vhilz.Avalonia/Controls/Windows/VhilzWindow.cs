using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Ursa.Controls;

namespace Vhilz.Avalonia.Theme.Controls;

/// <summary>
/// 使用 Vhilz 自有主题的 Ursa 窗口，支持标题栏内容与内嵌对话框。
/// </summary>
/// <remarks>
/// 标题栏使用继承的 LeftContent、TitleBarContent、RightContent。
/// 拖动、缩放和系统按钮行为复用 Ursa/Avalonia 的窗口装饰体系，通过 WindowDecorationsTheme 配置外观。
/// 异步关闭确认沿用 UrsaWindow.CanClose；模板应绑定继承的各按钮可见性属性。
/// </remarks>
[TemplatePart(PartTitleBar, typeof(Ursa.Controls.TitleBar))]
[TemplatePart(PART_DialogHost, typeof(Ursa.Controls.OverlayDialogHost))]
public class VhilzWindow : UrsaWindow
{
    /// <summary>
    /// 可选标题栏部件，供 Ursa 基类处理系统窗口按钮的避让。
    /// </summary>
    public const string PartTitleBar = "PART_TitleBar";

    /// <summary>
    /// 标题栏内边距属性，默认四边为零。
    /// </summary>
    public static readonly StyledProperty<Thickness> TitleBarPaddingProperty =
        AvaloniaProperty.Register<VhilzWindow, Thickness>(nameof(TitleBarPadding));

    /// <summary>
    /// 标题栏背景属性，默认 null，由主题决定显示效果。
    /// </summary>
    public static readonly StyledProperty<IBrush?> TitleBarBackgroundProperty =
        AvaloniaProperty.Register<VhilzWindow, IBrush?>(nameof(TitleBarBackground));

    private Ursa.Controls.OverlayDialogHost? _dialogHost;

    /// <summary>
    /// 获取或设置标题栏内边距。默认四边为零，需要在模板中绑定到标题栏容器。
    /// </summary>
    public Thickness TitleBarPadding
    {
        get => GetValue(TitleBarPaddingProperty);
        set => SetValue(TitleBarPaddingProperty, value);
    }

    /// <summary>
    /// 获取或设置标题栏背景。默认 null，需要在模板中绑定到标题栏背景层。
    /// </summary>
    public IBrush? TitleBarBackground
    {
        get => GetValue(TitleBarBackgroundProperty);
        set => SetValue(TitleBarBackgroundProperty, value);
    }

    /// <inheritdoc />
    protected override Type StyleKeyOverride => typeof(VhilzWindow);

    /// <inheritdoc />
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        ClearDialogHost();

        base.OnApplyTemplate(e);
        _dialogHost = e.NameScope.Find<Ursa.Controls.OverlayDialogHost>(PART_DialogHost);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TemplateProperty)
        {
            // Template = null 不会触发 OnApplyTemplate，仍应立即移除旧宿主。
            ClearDialogHost();
        }
    }

    private void ClearDialogHost()
    {
        // Ursa 2.2.0 只添加新宿主；必须在基类接管新模板之前移除旧宿主。
        if (_dialogHost is null) return;
        LogicalChildren.Remove(_dialogHost);
        _dialogHost = null;
    }
}
