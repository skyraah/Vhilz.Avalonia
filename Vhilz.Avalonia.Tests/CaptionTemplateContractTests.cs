using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Vhilz.Avalonia.Theme;
using Vhilz.Avalonia.Theme.Controls;
using Xunit;

namespace Vhilz.Avalonia.Tests;

public class CaptionTemplateContractTests {
    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void GradientBackgroundReachesVisibleTemplateRendering(bool light) {
        var gradient = new LinearGradientBrush {
            GradientStops = [new GradientStop(Colors.Red, 0), new GradientStop(Colors.Blue, 1)]
        };
        var button = CreateButton();
        button.Background = gradient;
        var window = CreateWindow(button, light);
        window.Show();
        try {
            var background = Assert.Single(button.GetVisualDescendants().OfType<Border>(),
                border => border.IsEffectivelyVisible && ReferenceEquals(border.Background, gradient));
            var drawing = new DrawingGroup();
            using (var context = drawing.Open()) background.Render(context);
            var fill = Assert.IsType<GeometryDrawing>(Assert.Single(drawing.Children));
            Assert.Same(gradient, fill.Brush);
            gradient.GradientStops[0].Color = Colors.Green;
            Assert.Equal(Colors.Green, Assert.IsAssignableFrom<IGradientBrush>(fill.Brush).GradientStops[0].Color);

            // 切回实色由原来的颜色动画绘制，不能再叠加一层同色背景。
            var surface = Assert.Single(button.GetVisualDescendants().OfType<CaptionSurface>());
            surface.Transitions = null;
            button.Background = Brushes.Purple;
            Assert.Null(background.Background);
            Assert.Equal(Colors.Purple, surface.SurfaceColor);
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void BorderOverrideReachesVisibleTemplateRendering(bool light) {
        var button = CreateButton();
        button.BorderBrush = Brushes.Red;
        button.BorderThickness = new Thickness(2);
        var window = CreateWindow(button, light);
        window.Show();
        try {
            var border = Assert.Single(button.GetVisualDescendants().OfType<Border>(),
                item => item.IsEffectivelyVisible && ReferenceEquals(item.BorderBrush, Brushes.Red));
            var drawing = new DrawingGroup();
            using (var context = drawing.Open()) border.Render(context);
            var shape = Assert.IsType<GeometryDrawing>(Assert.Single(drawing.Children));
            Assert.Same(Brushes.Red, shape.Pen!.Brush);
            Assert.Equal(2, shape.Pen.Thickness);

            // 非对称厚度也交给框架 Border，避免自写描边只支持统一线宽。
            button.BorderThickness = new Thickness(1, 2, 3, 4);
            Assert.Equal(button.BorderThickness, border.BorderThickness);
            window.UpdateLayout();
            drawing = new DrawingGroup();
            using (var context = drawing.Open()) border.Render(context);
            Assert.Contains(drawing.Children.OfType<GeometryDrawing>(), item => ReferenceEquals(item.Brush, Brushes.Red));
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaTheory]
    [InlineData(false, HorizontalAlignment.Left, VerticalAlignment.Top, 0, 0)]
    [InlineData(true, HorizontalAlignment.Left, VerticalAlignment.Top, 0, 0)]
    [InlineData(false, HorizontalAlignment.Right, VerticalAlignment.Bottom, 36, 22)]
    [InlineData(true, HorizontalAlignment.Right, VerticalAlignment.Bottom, 36, 22)]
    public void ContentAlignmentControlsActualPosition(bool light, HorizontalAlignment horizontal,
        VerticalAlignment vertical, double x, double y) {
        var content = new Border { Width = 10, Height = 10 };
        var button = CreateButton();
        button.Content = content;
        button.HorizontalContentAlignment = horizontal;
        button.VerticalContentAlignment = vertical;
        var window = CreateWindow(button, light);
        window.Show();
        try {
            Assert.Equal(new Point(x, y), content.TranslatePoint(default, button));
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void ExternalMarginDoesNotShrinkTheInternalSurface(bool light) {
        var button = CreateButton();
        button.Margin = new Thickness(4);
        var window = CreateWindow(button, light);
        window.Show();
        try {
            var surface = Assert.Single(button.GetVisualDescendants().OfType<CaptionSurface>());
            Assert.Equal(button.Bounds.Size, surface.Bounds.Size);
            Assert.Equal(default, surface.Margin);
            button.Margin = new Thickness(8);
            window.UpdateLayout();
            Assert.Equal(button.Bounds.Size, surface.Bounds.Size);
        }
        finally {
            window.Close();
        }
    }

    private static Button CreateButton() {
        var theme = new VhilzTheme();
        Assert.True(theme.TryGetResource(ResourceKeys.CaptionButton.Theme, null, out var resource));
        return new Button {
            Theme = Assert.IsType<ControlTheme>(resource), Width = 46, Height = 32,
            Margin = default, Padding = default
        };
    }

    private static Window CreateWindow(Button button, bool light) => new() {
        Content = button, RequestedThemeVariant = light ? ThemeVariant.Light : ThemeVariant.Dark
    };
}
