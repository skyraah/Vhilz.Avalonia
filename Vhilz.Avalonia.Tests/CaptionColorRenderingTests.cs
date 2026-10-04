using System.Reflection;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Fluid.Avalonia.Acrylic;
using SkiaSharp;
using Vhilz.Avalonia.Theme;
using Xunit;

namespace Vhilz.Avalonia.Tests;

public class CaptionColorRenderingTests {
    [AvaloniaFact]
    public void FirstVisibleColorStepDoesNotRecolorTheBackdropAbruptly() {
        var theme = new VhilzTheme();
        Assert.True(theme.TryGetResource("Vhilz.CaptionButton.Theme", null, out var resource));
        var button = new Button { Theme = Assert.IsType<ControlTheme>(resource), Width = 46, Height = 32 };
        var window = new Window { Content = button };
        window.Show();
        try {
            var surface = Assert.Single(button.GetVisualDescendants().OfType<AcrylicSurface>());
            var transition = Assert.Single(surface.Transitions!.OfType<ColorTransition>());
            var property = Assert.IsType<StyledProperty<Color>>(transition.Property);
            surface.Transitions = null;
            var backdrop = new SKColor(35, 105, 190);
            surface.SetValue(property, Color.FromArgb(0, 229, 72, 77));
            var before = RenderSurfaceOverlay(surface, backdrop);
            surface.SetValue(property, Color.FromArgb(1, 229, 72, 77));
            var after = RenderSurfaceOverlay(surface, backdrop);
            var change = MaxChannelChange(before, after);
            Assert.True(change <= 2, $"Alpha 0→1 引起最大通道变化 {change}：{before} → {after}");
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void HoverEasingDoesNotSpendMostOfItsChangeAtTheStart() {
        var theme = new VhilzTheme();
        Assert.True(theme.TryGetResource("Vhilz.CaptionButton.Theme", null, out var resource));
        var button = new Button { Theme = Assert.IsType<ControlTheme>(resource) };
        var window = new Window { Content = button };
        window.Show();
        try {
            var surface = Assert.Single(button.GetVisualDescendants().OfType<AcrylicSurface>());
            var transition = Assert.Single(surface.Transitions!.OfType<ColorTransition>());
            var firstFrameProgress = transition.Easing.Ease(16 / transition.Duration.TotalMilliseconds);
            Assert.Equal(TimeSpan.FromMilliseconds(120), transition.Duration);
            Assert.True(firstFrameProgress <= 0.1, $"前 16 ms 已完成 {firstFrameProgress:P1} 的颜色变化");
        }
        finally {
            window.Close();
        }
    }

    // Headless 不绘制 FAA 材质；直接调用当前依赖的实际 Skia 染色代码，检查起始像素连续性。
    // 反射限定在测试中，依赖升级后若内部接口变化则明确失败，避免用自制混色公式掩盖上游行为。
    private static SKColor RenderSurfaceOverlay(AcrylicSurface surface, SKColor backdrop) {
        const BindingFlags instanceMethods = BindingFlags.Instance | BindingFlags.NonPublic;
        var parameters =
            typeof(AcrylicSurface).GetMethod("CreateDrawParameters", instanceMethods)!.Invoke(surface, null);
        var assembly = typeof(AcrylicSurface).Assembly;
        var operationType = assembly.GetType("Fluid.Avalonia.Acrylic.AcrylicDrawOperation", true)!;
        var passType = assembly.GetType("Fluid.Avalonia.Acrylic.AcrylicDrawPass", true)!;
        using var operation = (IDisposable)Activator.CreateInstance(operationType,
            new Rect(0, 0, 8, 8), parameters, null, Enum.Parse(passType, "Lens"))!;
        using var bitmap = new SKBitmap(8, 8);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(backdrop);
        operationType.GetMethod("DrawSurfaceOverlay", instanceMethods)!.Invoke(operation,
            new object[] { canvas, new SKRect(0, 0, 8, 8) });
        canvas.Flush();
        return bitmap.GetPixel(4, 4);
    }

    private static int MaxChannelChange(SKColor a, SKColor b) =>
        Math.Max(Math.Abs(a.Red - b.Red), Math.Max(Math.Abs(a.Green - b.Green), Math.Abs(a.Blue - b.Blue)));
}
