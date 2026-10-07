using Avalonia;
using Avalonia.Media;
using Lucide.Avalonia;

namespace Vhilz.Avalonia.Theme.Controls;

/// <summary>
/// 使用 Lucide 的布局与描边规则绘制自定义 SVG 路径。
/// </summary>
/// <remarks>
/// 路径采用 24×24 坐标系，以 Foreground 绘制圆角描边，不填充。
/// Data 接收 Geometry 或 XAML 中的路径字符串，不解析 SVG 文件或 XML；继承的 Kind 不参与绘制。
/// </remarks>
public sealed class GeometryIcon : LucideIcon
{
    public static readonly StyledProperty<Geometry?> DataProperty =
        AvaloniaProperty.Register<GeometryIcon, Geometry?>(nameof(Data));

    private readonly Pen _pen = new(null, lineCap: PenLineCap.Round, lineJoin: PenLineJoin.Round);

    /// <summary>
    /// 图标的路径几何；为空时不绘制内容。
    /// </summary>
    public Geometry? Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    static GeometryIcon()
    {
        AffectsRender<GeometryIcon>(DataProperty);
    }

    public override void Render(DrawingContext context)
    {
        if (Data is not { } geometry)
            return;

        // 绘制沿用 Lucide 的透明边界、24 单位坐标变换和空填充圆角描边。
        _pen.Brush = Foreground;
        _pen.Thickness = StrokeWidth;
        context.DrawRectangle(Brushes.Transparent, null, new Rect(Bounds.Size));
        if (IsSet(SizeProperty))
        {
            var scale = Size / 24;
            using (context.PushTransform(Matrix.CreateScale(scale, scale)))
                context.DrawGeometry(null, _pen, geometry);
        }
        else
        {
            context.DrawGeometry(null, _pen, geometry);
        }
    }
}
