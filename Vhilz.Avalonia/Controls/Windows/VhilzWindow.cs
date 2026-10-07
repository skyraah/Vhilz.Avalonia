using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Vhilz.Avalonia.Theme.Platforms;

namespace Vhilz.Avalonia.Theme.Controls;

/// <summary>
/// 直接基于 Avalonia Window 的主题窗口，支持自定义标题栏内容。
/// </summary>
/// <remarks>
/// 拖动、缩放与窗口按钮交由 Avalonia 的窗口装饰体系处理。
/// 关闭拦截使用 Closing 事件，模态子窗口使用 ShowDialog。
/// </remarks>
[TemplatePart(PartTitleBar, typeof(TitleBar))]
[TemplatePart(PartTitleBarInset, typeof(Border))]
[TemplatePart(PartFullscreenTitleBarInset, typeof(Border))]
public class VhilzWindow : Window {
    /// <summary>
    /// 窗口模板的标题栏部件。
    /// </summary>
    public const string PartTitleBar = "PART_TitleBar";

    /// <summary>用于避让自绘窗口按钮的标题栏容器。</summary>
    public const string PartTitleBarInset = "PART_TitleBarInset";

    /// <summary>全屏标题栏展开时为正文保留空间的模板部件。</summary>
    public const string PartFullscreenTitleBarInset = "PART_FullscreenTitleBarInset";

    private Border? _titleBarInset;
    private Control? _captionButtons;
    private readonly FullscreenTitleBarController _fullscreenTitleBar;
    private IFullscreenTransition? _fullscreenTransition;

    /// <summary>初始化窗口标题栏布局同步。</summary>
    public VhilzWindow() {
        _fullscreenTitleBar = new FullscreenTitleBarController(this);
        Opened += (_, _) => _fullscreenTransition ??= FullscreenTransition.TryAttach(this);
        Closed += (_, _) => {
            _fullscreenTitleBar.ObserveFullscreenPopover(null);
            _fullscreenTitleBar.SetInset(null);
            SetCaptionButtons(null);
            _fullscreenTransition?.Dispose();
            _fullscreenTransition = null;
        };
    }

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

    /// <summary>标题栏左侧内容。</summary>
    public static readonly StyledProperty<object?> LeftContentProperty =
        TitleBar.LeftContentProperty.AddOwner<VhilzWindow>();

    /// <summary>标题文字之后的内容。</summary>
    public static readonly StyledProperty<object?> CenterContentProperty =
        TitleBar.CenterContentProperty.AddOwner<VhilzWindow>();

    /// <summary>标题栏右侧内容。</summary>
    public static readonly StyledProperty<object?> RightContentProperty =
        TitleBar.RightContentProperty.AddOwner<VhilzWindow>();

    /// <summary>自定义标题栏是否显示，不控制系统窗口装饰。</summary>
    public static readonly StyledProperty<bool> IsTitleBarVisibleProperty =
        AvaloniaProperty.Register<VhilzWindow, bool>(nameof(IsTitleBarVisible), true);

    /// <summary>标题栏的附加外边距。</summary>
    public static readonly StyledProperty<Thickness> TitleBarMarginProperty =
        AvaloniaProperty.Register<VhilzWindow, Thickness>(nameof(TitleBarMargin));

    /// <summary>自绘装饰中全屏按钮是否显示。</summary>
    public static readonly StyledProperty<bool> IsFullScreenButtonVisibleProperty =
        AvaloniaProperty.Register<VhilzWindow, bool>(nameof(IsFullScreenButtonVisible));

    /// <summary>自绘装饰中最小化按钮是否显示。</summary>
    public static readonly StyledProperty<bool> IsMinimizeButtonVisibleProperty =
        AvaloniaProperty.Register<VhilzWindow, bool>(nameof(IsMinimizeButtonVisible), true);

    /// <summary>自绘装饰中最大化与还原按钮是否显示。</summary>
    public static readonly StyledProperty<bool> IsRestoreButtonVisibleProperty =
        AvaloniaProperty.Register<VhilzWindow, bool>(nameof(IsRestoreButtonVisible), true);

    /// <summary>自绘装饰中关闭按钮是否显示。</summary>
    public static readonly StyledProperty<bool> IsCloseButtonVisibleProperty =
        AvaloniaProperty.Register<VhilzWindow, bool>(nameof(IsCloseButtonVisible), true);

    /// <summary>获取或设置全屏按钮可见性；平台仍决定是否支持该按钮。</summary>
    public bool IsFullScreenButtonVisible {
        get => GetValue(IsFullScreenButtonVisibleProperty);
        set => SetValue(IsFullScreenButtonVisibleProperty, value);
    }

    /// <summary>获取或设置最小化按钮可见性；可用性由 CanMinimize 决定。</summary>
    public bool IsMinimizeButtonVisible {
        get => GetValue(IsMinimizeButtonVisibleProperty);
        set => SetValue(IsMinimizeButtonVisibleProperty, value);
    }

    /// <summary>获取或设置最大化与还原按钮可见性；可用性由 Avalonia 决定。</summary>
    public bool IsRestoreButtonVisible {
        get => GetValue(IsRestoreButtonVisibleProperty);
        set => SetValue(IsRestoreButtonVisibleProperty, value);
    }

    /// <summary>获取或设置关闭按钮可见性。</summary>
    public bool IsCloseButtonVisible {
        get => GetValue(IsCloseButtonVisibleProperty);
        set => SetValue(IsCloseButtonVisibleProperty, value);
    }

    /// <summary>获取或设置标题栏左侧内容。</summary>
    public object? LeftContent {
        get => GetValue(LeftContentProperty);
        set => SetValue(LeftContentProperty, value);
    }

    /// <summary>获取或设置标题文字之后的内容。</summary>
    public object? CenterContent {
        get => GetValue(CenterContentProperty);
        set => SetValue(CenterContentProperty, value);
    }

    /// <summary>获取或设置标题栏右侧内容。</summary>
    public object? RightContent {
        get => GetValue(RightContentProperty);
        set => SetValue(RightContentProperty, value);
    }

    /// <summary>获取或设置自定义标题栏是否显示。</summary>
    public bool IsTitleBarVisible {
        get => GetValue(IsTitleBarVisibleProperty);
        set => SetValue(IsTitleBarVisibleProperty, value);
    }

    /// <summary>获取或设置标题栏的附加外边距。</summary>
    public Thickness TitleBarMargin {
        get => GetValue(TitleBarMarginProperty);
        set => SetValue(TitleBarMarginProperty, value);
    }

    /// <summary>标题文字显示开关，默认显示；独立于整个标题栏的可见性。</summary>
    public static readonly StyledProperty<bool> IsTitleVisibleProperty =
        TitleBar.IsTitleVisibleProperty.AddOwner<VhilzWindow>();

    /// <summary>标题文字在可用区域内的对齐方式，默认居左。</summary>
    public static readonly StyledProperty<TextAlignment> TitleAlignmentProperty =
        TitleBar.TitleAlignmentProperty.AddOwner<VhilzWindow>();

    /// <summary>获取或设置标题文字显示开关，不改变系统窗口标题及内容槽。</summary>
    public bool IsTitleVisible {
        get => GetValue(IsTitleVisibleProperty);
        set => SetValue(IsTitleVisibleProperty, value);
    }

    /// <summary>获取或设置标题对齐方式，支持 Left 和 Center。</summary>
    public TextAlignment TitleAlignment {
        get => GetValue(TitleAlignmentProperty);
        set => SetValue(TitleAlignmentProperty, value);
    }

    /// <summary>
    /// 获取或设置标题栏内边距。默认四边为零，需要在模板中绑定到标题栏容器。
    /// </summary>
    public Thickness TitleBarPadding {
        get => GetValue(TitleBarPaddingProperty);
        set => SetValue(TitleBarPaddingProperty, value);
    }

    /// <summary>
    /// 获取或设置标题栏背景。默认 null，需要在模板中绑定到标题栏背景层。
    /// </summary>
    public IBrush? TitleBarBackground {
        get => GetValue(TitleBarBackgroundProperty);
        set => SetValue(TitleBarBackgroundProperty, value);
    }

    /// <inheritdoc />
    protected override Type StyleKeyOverride => typeof(VhilzWindow);

    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e) {
        base.OnKeyDown(e);
        // 内容控件先处理 Esc（例如关闭弹层）；未消费时为全屏提供独立于装饰的退出入口。
        if (!e.Handled && e.Key == Key.Escape && e.KeyModifiers == KeyModifiers.None &&
            WindowState == WindowState.FullScreen) {
            WindowState = WindowState.Normal;
            e.Handled = true;
        }
    }

    /// <inheritdoc />
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);
        _titleBarInset = e.NameScope.Find<Border>(PartTitleBarInset);
        _fullscreenTitleBar.SetInset(e.NameScope.Find<Border>(PartFullscreenTitleBarInset));
        UpdateTitleBarInset();
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        if (change.Property == WindowStateProperty)
            _fullscreenTransition?.BeforeStateChange();
        else if (change.Property == IsVisibleProperty && !IsVisible)
            _fullscreenTransition?.BeforeHide();
        base.OnPropertyChanged(change);
        if (change.Property == WindowStateProperty || change.Property == IsExtendedIntoWindowDecorationsProperty)
            UpdateTitleBarInset();
        if ((change.Property == WindowStateProperty && WindowState != WindowState.FullScreen) ||
            (change.Property == IsVisibleProperty && !IsVisible)) {
            _fullscreenTitleBar.Reset();
        }

        if (change.Property == TemplateProperty) {
            _titleBarInset = null;
            _fullscreenTitleBar.SetInset(null);
        }
    }

    // 自有模板主动登记部件，避免按名称搜索上游私有宿主的逻辑或视觉子树。
    internal void SetCaptionButtons(Control? buttons) {
        if (ReferenceEquals(_captionButtons, buttons)) return;
        if (_captionButtons is not null) _captionButtons.PropertyChanged -= OnCaptionButtonsChanged;
        _captionButtons = buttons;
        if (buttons is not null) buttons.PropertyChanged += OnCaptionButtonsChanged;
        UpdateTitleBarInset();
    }

    internal void ClearCaptionButtons(Control host) {
        if (ReferenceEquals(_captionButtons, host)) SetCaptionButtons(null);
    }

    internal void ClearFullscreenPopover(Control host) => _fullscreenTitleBar.ClearPopover(host);

    internal void SetFullscreenPopover(Control? popover) => _fullscreenTitleBar.ObserveFullscreenPopover(popover);

    private void OnCaptionButtonsChanged(object? sender, AvaloniaPropertyChangedEventArgs e) {
        if (e.Property == BoundsProperty || e.Property == MarginProperty ||
            e.Property == IsVisibleProperty)
            UpdateTitleBarInset();
    }

    private void UpdateTitleBarInset() {
        if (_titleBarInset is null) return;
        var hasOverlap = IsExtendedIntoWindowDecorations && WindowState != WindowState.FullScreen &&
                         _captionButtons is { IsEffectivelyVisible: true };
        _titleBarInset.Padding = hasOverlap
            ? new Thickness(0, 0, _captionButtons!.Bounds.Width + _captionButtons.Margin.Left + _captionButtons.Margin.Right, 0)
            : default;
        _titleBarInset.MinHeight = hasOverlap ? _captionButtons!.Bounds.Height : 0;
    }
}
