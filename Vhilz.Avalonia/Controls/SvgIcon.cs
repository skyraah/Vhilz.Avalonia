using Avalonia;
using Avalonia.Controls.Documents;
using Avalonia.Data;
using Avalonia.Media;
using SvgControl = Avalonia.Svg.Skia.Svg;

namespace Vhilz.Avalonia.Theme.Controls;

/// <summary>
/// 加载 SVG 文件或资源，以正方形图标布局保留原始比例与多色内容。
/// </summary>
/// <remarks>
/// Path 接收本地文件路径或 avares URI；Source 接收 SVG XML。
/// SVG 的 currentColor 默认跟随 Foreground 的纯色值，非纯色画笔回退为黑色。
/// </remarks>
public sealed class SvgIcon : SvgControl
{
    public static readonly StyledProperty<double> SizeProperty =
        AvaloniaProperty.Register<SvgIcon, double>(nameof(Size), 24,
            validate: value => double.IsFinite(value) && value >= 0);

    public static readonly StyledProperty<IBrush?> ForegroundProperty =
        TextElement.ForegroundProperty.AddOwner<SvgIcon>();

    public double Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    static SvgIcon()
    {
        AffectsMeasure<SvgIcon>(SizeProperty);
        AffectsRender<SvgIcon>(SizeProperty);
    }

    public SvgIcon() : this((Uri?)null) { }

    public SvgIcon(Uri? baseUri) : base(baseUri!) => BindForeground();

    public SvgIcon(IServiceProvider serviceProvider) : base(serviceProvider) => BindForeground();

    protected override global::Avalonia.Size MeasureOverride(global::Avalonia.Size availableSize) => new(Size, Size);

    protected override global::Avalonia.Size ArrangeOverride(global::Avalonia.Size finalSize)
    {
        base.ArrangeOverride(new global::Avalonia.Size(Size, Size));
        return new global::Avalonia.Size(Size, Size);
    }

    private void BindForeground()
    {
        // 跟踪画笔的 Color，使主题资源在同一画笔对象内变色时也能同步 currentColor。
        Bind(CurrentColorProperty, new Binding("Foreground.Color")
        {
            Source = this,
            FallbackValue = Colors.Black,
            TargetNullValue = Colors.Black
        });
    }
}
