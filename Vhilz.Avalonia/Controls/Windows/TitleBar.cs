using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Vhilz.Avalonia.Theme.Controls;

/// <summary>
/// 使用 Vhilz 主题的标题栏，提供独立的内容槽与标题显示设置。
/// </summary>
public class TitleBar : TemplatedControl
{
    /// <summary>标题栏左侧内容。</summary>
    public static readonly StyledProperty<object?> LeftContentProperty =
        AvaloniaProperty.Register<TitleBar, object?>(nameof(LeftContent));

    /// <summary>标题文字之后的内容。</summary>
    public static readonly StyledProperty<object?> CenterContentProperty =
        AvaloniaProperty.Register<TitleBar, object?>(nameof(CenterContent));

    /// <summary>标题栏右侧内容。</summary>
    public static readonly StyledProperty<object?> RightContentProperty =
        AvaloniaProperty.Register<TitleBar, object?>(nameof(RightContent));

    /// <summary>获取或设置左侧内容。</summary>
    public object? LeftContent
    {
        get => GetValue(LeftContentProperty);
        set => SetValue(LeftContentProperty, value);
    }

    /// <summary>获取或设置标题文字之后的内容。</summary>
    public object? CenterContent
    {
        get => GetValue(CenterContentProperty);
        set => SetValue(CenterContentProperty, value);
    }

    /// <summary>获取或设置右侧内容。</summary>
    public object? RightContent
    {
        get => GetValue(RightContentProperty);
        set => SetValue(RightContentProperty, value);
    }

    /// <summary>控制窗口标题文字是否显示，默认显示；不影响内容槽和窗口按钮。</summary>
    public static readonly StyledProperty<bool> IsTitleVisibleProperty =
        AvaloniaProperty.Register<TitleBar, bool>(nameof(IsTitleVisible), true);

    /// <summary>标题在可用区域内的文字对齐方式，支持居左和居中，默认居左。</summary>
    public static readonly StyledProperty<TextAlignment> TitleAlignmentProperty =
        AvaloniaProperty.Register<TitleBar, TextAlignment>(nameof(TitleAlignment), TextAlignment.Left,
            validate: value => value is TextAlignment.Left or TextAlignment.Center);

    /// <summary>获取或设置标题文字显示开关；空标题仍隐藏。</summary>
    public bool IsTitleVisible
    {
        get => GetValue(IsTitleVisibleProperty);
        set => SetValue(IsTitleVisibleProperty, value);
    }

    /// <summary>获取或设置标题文字在可用区域内的对齐方式。</summary>
    public TextAlignment TitleAlignment
    {
        get => GetValue(TitleAlignmentProperty);
        set => SetValue(TitleAlignmentProperty, value);
    }

    /// <inheritdoc />
    protected override Type StyleKeyOverride => typeof(TitleBar);
}
