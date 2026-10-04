using Avalonia;
using Avalonia.Media;
using Lucide.Avalonia;

namespace Vhilz.Avalonia.Theme.Controls;

// 仅供窗口模板使用，不扩展主题库的公开控件 API。
internal sealed class CaptionGeometryIcon : LucideIcon
{
    public static readonly StyledProperty<Geometry?> DataProperty =
        AvaloniaProperty.Register<CaptionGeometryIcon, Geometry?>(nameof(Data));

    private readonly Pen _pen = new(null, lineCap: PenLineCap.Round, lineJoin: PenLineJoin.Round);

    public Geometry? Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    static CaptionGeometryIcon()
    {
        AffectsRender<CaptionGeometryIcon>(DataProperty);
    }

    public override void Render(DrawingContext context)
    {
        if (Data is not { } geometry)
            return;

        // Lucide 0.2.24 没有公开的自定义路径入口。继承其尺寸、布局和前景色契约，
        // 绘制顺序与其 Render 一致：透明边界、24 单位坐标变换、空填充圆角描边。
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
