using System.Reflection;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;
using Vhilz.Avalonia.Theme.Controls;
using Xunit;

namespace Vhilz.Avalonia.Tests;

public class CaptionSurfaceTests {
    [AvaloniaFact]
    public void DecorationSurfaceTracksPointerProximityAndDetachesFromRoot() {
        var window = new Window();
        window.Show();
        var root = Assert.IsAssignableFrom<Control>(window.GetVisualAncestors().Last());
        var surface = new CaptionSurface {
            RevealBorderEnabled = true, RevealProximityDistance = 15, UseLayoutRounding = false
        };
        var children = GetVisualChildren(root);
        // 复现装饰层与 Window 为视觉兄弟的结构，不把按钮搬入窗口内容区。
        children.Add(surface);
        surface.Measure(new Size(46, 32));
        surface.Arrange(new Rect(100, 0, 46, 32));
        try {
            Assert.Null(TopLevel.GetTopLevel(surface));
            Move(root, surface, new Point(110, 10));
            Assert.Equal(new Point(10, 10), surface.DecorationRevealPosition);
            Assert.Equal(1, surface.DecorationRevealIntensity);
            Assert.Equal(new RelativePoint(10, 10, RelativeUnit.Absolute), RenderReveal(surface).Center);

            Move(root, surface, new Point(130, 20));
            Assert.Equal(new Point(30, 20), surface.DecorationRevealPosition);
            Assert.Equal(new RelativePoint(30, 20, RelativeUnit.Absolute), RenderReveal(surface).Center);
            Move(root, window, new Point(92.5, 10));
            Assert.Equal(0.5, surface.DecorationRevealIntensity);
            // 指针不动，运行时参数与几何改变也必须立即使用新的接近范围。
            surface.RevealProximityDistance = 30;
            Assert.Equal(0.75, surface.DecorationRevealIntensity);
            surface.Arrange(new Rect(107.5, 0, 46, 32));
            Assert.Equal(new Point(-15, 10), surface.DecorationRevealPosition);
            Assert.Equal(0.5, surface.DecorationRevealIntensity);
            surface.Arrange(new Rect(100, 0, 46, 32));
            surface.RevealProximityDistance = 15;
            Assert.Equal(0.5, surface.DecorationRevealIntensity);
            Move(root, window, new Point(70, 10));
            Assert.Equal(0, surface.DecorationRevealIntensity);

            Move(root, surface, new Point(110, 10));
            surface.RevealBorderEnabled = false;
            Assert.Equal(0, surface.DecorationRevealIntensity);
            surface.RevealBorderEnabled = true;
            Assert.Equal(1, surface.DecorationRevealIntensity);
            var exit = PointerEvent(root, root, InputElement.PointerExitedEvent, default);
            root.RaiseEvent(exit);
            Assert.Equal(0, surface.DecorationRevealIntensity);
            surface.RevealProximityDistance = 30;
            Assert.Equal(0, surface.DecorationRevealIntensity);

            children.Remove(surface);
            Move(root, window, new Point(110, 10));
            Assert.Equal(default, surface.DecorationRevealPosition);
            Assert.Equal(0, surface.DecorationRevealIntensity);
        }
        finally {
            children.Remove(surface);
            window.Close();
        }
    }

    [AvaloniaFact]
    public void DecorationRenderHasNoPlaceholderFillAndUsesAnimatedStateColor() {
        var surface = new CaptionSurface();
        surface.Measure(new Size(46, 32));
        surface.Arrange(new Rect(0, 0, 46, 32));
        Assert.Null(TopLevel.GetTopLevel(surface));
        foreach (var color in new[] { Colors.Transparent, Color.Parse("#01E5484D"), Color.Parse("#BFE5484D") }) {
            surface.SurfaceColor = color;
            var drawing = new DrawingGroup();
            using (var context = drawing.Open())
                surface.Render(context);
            var fill = Assert.IsType<GeometryDrawing>(Assert.Single(drawing.Children));
            Assert.Equal(color, Assert.IsAssignableFrom<ISolidColorBrush>(fill.Brush).Color);
            Assert.Null(fill.Pen);
        }
    }

    private static void Move(Visual root, Interactive source, Point point) =>
        source.RaiseEvent(PointerEvent(root, source, InputElement.PointerMovedEvent, point));

    private static IRadialGradientBrush RenderReveal(CaptionSurface surface) {
        var drawing = new DrawingGroup();
        using (var context = drawing.Open())
            surface.Render(context);
        Assert.Equal(2, drawing.Children.Count);
        var reveal = Assert.IsType<GeometryDrawing>(drawing.Children[1]);
        return Assert.IsAssignableFrom<IRadialGradientBrush>(reveal.Pen!.Brush);
    }

    private static PointerEventArgs PointerEvent(Visual root, Interactive source,
        RoutedEvent<PointerEventArgs> routedEvent, Point point) =>
        new(routedEvent, source, new global::Avalonia.Input.Pointer(1, PointerType.Mouse, true), root, point, 0,
                new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.Other), KeyModifiers.None)
            { Handled = true };

    private static IAvaloniaList<Visual> GetVisualChildren(Visual root) =>
        (IAvaloniaList<Visual>)typeof(Visual)
            .GetProperty("VisualChildren", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(root)!;
}
