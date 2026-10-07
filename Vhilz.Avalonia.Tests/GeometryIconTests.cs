using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Lucide.Avalonia;
using Vhilz.Avalonia.Theme.Controls;
using Xunit;

namespace Vhilz.Avalonia.Tests;

public class GeometryIconTests
{
    [AvaloniaTheory]
    [InlineData(12)]
    [InlineData(16)]
    [InlineData(20)]
    [InlineData(24)]
    public void DrawingCommandsMatchInstalledLucideAtTheSameSize(double size)
    {
        var lucide = new LucideIcon { Kind = LucideIconKind.Square, Size = size };
        var custom = new GeometryIcon
        {
            Data = Geometry.Parse(LucideIconKind.Square.GetGeometryData()),
            Size = size
        };
        foreach (var brush in new[] { Brushes.White, Brushes.Black })
        foreach (var width in new[] { 2d, 1.5d })
        {
            lucide.Foreground = custom.Foreground = brush;
            lucide.StrokeWidth = custom.StrokeWidth = width;
            AssertDrawingsMatch(Capture(lucide), Capture(custom));
            Assert.Equal(lucide.Bounds, custom.Bounds);
            Assert.Equal(lucide.DesiredSize, custom.DesiredSize);
        }
    }

    [AvaloniaFact]
    public void DefaultSizeAndClearedGeometryFollowDirectDrawingContract()
    {
        var custom = new GeometryIcon
        {
            Data = Geometry.Parse(LucideIconKind.Square.GetGeometryData()),
            Foreground = Brushes.White
        };
        var lucide = new LucideIcon { Kind = LucideIconKind.Square, Foreground = Brushes.White };
        AssertDrawingsMatch(Capture(lucide), Capture(custom));
        custom.Data = null;
        Assert.Empty(Capture(custom).Children);
    }

    private static DrawingGroup Capture(Control control)
    {
        control.Measure(new Size(40, 40));
        control.Arrange(new Rect(0, 0, 40, 40));
        var drawing = new DrawingGroup();
        using (var context = drawing.Open())
            control.Render(context);
        return drawing;
    }

    // 对比实际 Lucide Render 生成的指令；这里不评价像素抗锯齿或真实窗口视觉。
    private static void AssertDrawingsMatch(Drawing expected, Drawing actual)
    {
        Assert.Equal(expected.GetType(), actual.GetType());
        Assert.Equal(expected.GetBounds(), actual.GetBounds());
        if (expected is DrawingGroup expectedGroup)
        {
            var actualGroup = Assert.IsType<DrawingGroup>(actual);
            Assert.Equal(expectedGroup.Transform?.Value, actualGroup.Transform?.Value);
            Assert.Equal(expectedGroup.Children.Count, actualGroup.Children.Count);
            for (var index = 0; index < expectedGroup.Children.Count; index++)
                AssertDrawingsMatch(expectedGroup.Children[index], actualGroup.Children[index]);
        }
        else
        {
            var expectedGeometry = Assert.IsType<GeometryDrawing>(expected);
            var actualGeometry = Assert.IsType<GeometryDrawing>(actual);
            Assert.Equal(expectedGeometry.Brush, actualGeometry.Brush);
            Assert.Equal(expectedGeometry.Geometry?.Bounds, actualGeometry.Geometry?.Bounds);
            if (expectedGeometry.Pen is { } pen)
            {
                Assert.NotNull(actualGeometry.Pen);
                Assert.Equal(pen.Brush, actualGeometry.Pen.Brush);
                Assert.Equal(pen.Thickness, actualGeometry.Pen.Thickness);
                Assert.Equal(pen.LineCap, actualGeometry.Pen.LineCap);
                Assert.Equal(pen.LineJoin, actualGeometry.Pen.LineJoin);
                Assert.Equal(pen.MiterLimit, actualGeometry.Pen.MiterLimit);
            }
            else
            {
                Assert.Null(actualGeometry.Pen);
            }
        }
    }
}
