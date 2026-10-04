using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Fluid.Avalonia.Acrylic;
using Lucide.Avalonia;
using SukiUI.Motion;
using Vhilz.Avalonia.Theme;
using Xunit;

namespace Vhilz.Avalonia.Tests;

public class CaptionButtonTests {
    [AvaloniaFact]
    public void CaptionButtonsFollowPointerThemeAndDisabledStates() {
        var theme = new VhilzTheme();
        Assert.True(theme.TryGetResource("Vhilz.WindowDecorations.Theme", null, out var resource));
        var decorations = Assert.IsType<ControlTheme>(resource);
        var setter = Assert.Single(decorations.Setters.OfType<Setter>(),
            item => item.Property == WindowDrawnDecorations.TemplateProperty);
        var template = Assert.IsAssignableFrom<IWindowDrawnDecorationsTemplate>(setter.Value).Build();
        var panel = new StackPanel();
        var names = new[] {
            "PART_MinimizeButton", "PART_MaximizeButton", "PART_FullScreenButton",
            "PART_CloseButton", "PART_PopoverFullScreenButton", "PART_PopoverCloseButton"
        };
        // 使用生产模板中的按钮，在独立宿主内测试输入，不依赖平台原生装饰。
        foreach (var name in names) {
            var button = Assert.IsType<Button>(template.NameScope.Find(name));
            Assert.IsType<StackPanel>(button.Parent).Children.Remove(button);
            button.Height = 32;
            panel.Children.Add(button);
        }

        var window = new Window { Width = 400, Height = 300, Content = panel };
        window.Show();
        try {
            foreach (var variant in new[] { ThemeVariant.Dark, ThemeVariant.Light }) {
                window.RequestedThemeVariant = variant;
                window.UpdateLayout();
                Assert.True(
                    theme.TryGetResource("Vhilz.CaptionButton.RevealBorder.Color", variant, out var revealColor));
                Assert.Equal(new AcrylicSurface().RevealBorderColor, Assert.IsType<Color>(revealColor));
                foreach (var button in panel.Children.Cast<Button>()) {
                    var close = button.Name!.Contains("Close");
                    var pointerOverKey = close
                        ? "Vhilz.CaptionButton.Close.PointerOver.BackgroundBrush"
                        : "Vhilz.CaptionButton.PointerOver.BackgroundBrush";
                    var pressedKey = close
                        ? "Vhilz.CaptionButton.Close.Pressed.BackgroundBrush"
                        : "Vhilz.CaptionButton.Pressed.BackgroundBrush";
                    var foreground = button.Foreground;
                    var surface = Assert.Single(button.GetVisualDescendants().OfType<AcrylicSurface>());
                    Assert.Equal(Assert.IsType<Color>(revealColor), surface.RevealBorderColor);
                    // 此测试检查目标状态；实际渐变与反向过程由独立测试驱动时钟验证。
                    foreach (var colorTransition in surface.Transitions!.OfType<ColorTransition>().ToArray())
                        surface.Transitions!.Remove(colorTransition);
                    button.Transitions = null;
                    var transition = Assert.IsType<TransformOperationsTransition>(Assert.Single(surface.Transitions!));
                    Assert.True(transition.Duration > TimeSpan.Zero);
                    Assert.IsType<SukiSpringEaseOut>(transition.Easing);
                    var bounds = button.Bounds;
                    var clickCount = 0;
                    void OnClick(object? sender, global::Avalonia.Interactivity.RoutedEventArgs args) => clickCount++;
                    button.Click += OnClick;
                    // 状态色由 FAA 绘制，内容不再叠加实色背景。
                    var presenter = Assert.IsType<ContentPresenter>(surface.Content);
                    Assert.Null(presenter.Background);
                    Assert.Equal(Colors.Transparent, surface.TintColor);
                    Assert.Equal(Colors.Transparent, surface.SurfaceColor);
                    var point = button.TranslatePoint(new Point(button.Bounds.Width / 2, button.Bounds.Height / 2),
                        window)!.Value;

                    window.MouseMove(point);
                    Assert.True(button.IsPointerOver);
                    AssertBrushResource(theme, variant, pointerOverKey, button.Background);
                    Assert.Equal(Assert.IsAssignableFrom<ISolidColorBrush>(button.Background).Color,
                        surface.SurfaceColor);
                    if (close) {
                        Assert.Equal(Colors.White, Assert.IsAssignableFrom<ISolidColorBrush>(button.Foreground).Color);
                        Assert.Same(button.Foreground, Assert.IsType<LucideIcon>(button.Content).Foreground);
                    }

                    window.MouseDown(point, MouseButton.Left);
                    Assert.True(button.IsPressed);
                    Assert.True(Assert
                        .IsAssignableFrom<ITransform>(surface.GetBaseValue(Visual.RenderTransformProperty).Value).Value
                        .M11 < 1);
                    Assert.Equal(bounds, button.Bounds);
                    AssertBrushResource(theme, variant, pressedKey, button.Background);
                    Assert.Equal(Assert.IsAssignableFrom<ISolidColorBrush>(button.Background).Color,
                        surface.SurfaceColor);
                    window.MouseUp(point, MouseButton.Left);
                    Assert.Equal(1, clickCount);
                    Assert.Equal(1d,
                        Assert.IsAssignableFrom<ITransform>(surface.GetBaseValue(Visual.RenderTransformProperty).Value)
                            .Value.M11);
                    AssertBrushResource(theme, variant, pointerOverKey, button.Background);
                    Assert.Equal(Assert.IsAssignableFrom<ISolidColorBrush>(button.Background).Color,
                        surface.SurfaceColor);

                    // 按住后移出应取消点击；缩放表面不能改变按钮的命中范围。
                    window.MouseDown(point, MouseButton.Left);
                    window.MouseMove(new Point(350, 280));
                    window.MouseUp(new Point(350, 280), MouseButton.Left);
                    Assert.Equal(1, clickCount);
                    Assert.False(button.IsPressed);
                    Assert.Equal(1d,
                        Assert.IsAssignableFrom<ITransform>(surface.GetBaseValue(Visual.RenderTransformProperty).Value)
                            .Value.M11);
                    window.MouseMove(point);
                    button.IsEnabled = false;
                    window.MouseDown(point, MouseButton.Left);
                    window.MouseUp(point, MouseButton.Left);
                    Assert.Equal(1, clickCount);
                    Assert.Equal(1d,
                        Assert.IsAssignableFrom<ITransform>(surface.GetBaseValue(Visual.RenderTransformProperty).Value)
                            .Value.M11);
                    button.Click -= OnClick;
                    Assert.Equal(Colors.Transparent,
                        Assert.IsAssignableFrom<ISolidColorBrush>(button.Background).Color);
                    Assert.Equal(Colors.Transparent, surface.SurfaceColor);
                    Assert.Same(foreground, button.Foreground);
                    Assert.True(button.Opacity < 1);
                    window.MouseMove(new Point(350, 280));
                    button.IsEnabled = true;
                    Assert.Equal(Colors.Transparent,
                        Assert.IsAssignableFrom<ISolidColorBrush>(button.Background).Color);
                    Assert.Equal(1d, button.Opacity);
                    Assert.Same(foreground, button.Foreground);
                }
            }

            var closeButton = panel.Children.Cast<Button>().Single(button => button.Name == "PART_CloseButton");
            var closeSurface = Assert.Single(closeButton.GetVisualDescendants().OfType<AcrylicSurface>());
            var closePoint = closeButton.TranslatePoint(new Point(20, 16), window)!.Value;
            window.MouseMove(closePoint);
            // 悬停期间切换主题和覆盖资源，应立即更新而无需移出重进。
            window.RequestedThemeVariant = ThemeVariant.Dark;
            AssertBrushResource(theme, ThemeVariant.Dark, "Vhilz.CaptionButton.Close.PointerOver.BackgroundBrush",
                closeButton.Background);
            Assert.Equal(Assert.IsAssignableFrom<ISolidColorBrush>(closeButton.Background).Color,
                closeSurface.SurfaceColor);
            var overrideBrush = new SolidColorBrush(Colors.Purple);
            window.Resources["Vhilz.CaptionButton.Close.PointerOver.BackgroundBrush"] = overrideBrush;
            Assert.Same(overrideBrush, closeButton.Background);
            Assert.Equal(Colors.Purple, closeSurface.SurfaceColor);
            overrideBrush.Color = Color.Parse("#80D92D20");
            Assert.Equal(overrideBrush.Color, closeSurface.SurfaceColor);
            window.MouseMove(new Point(350, 280));
            Assert.Equal(Colors.Transparent, Assert.IsAssignableFrom<ISolidColorBrush>(closeButton.Background).Color);
            Assert.Equal(Colors.Transparent, closeSurface.SurfaceColor);
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaFact]
    public async Task CaptionSurfaceColorTransitionsThroughIntermediateColorsAndReverses() {
        var theme = new VhilzTheme();
        Assert.True(theme.TryGetResource("Vhilz.CaptionButton.Theme", null, out var resource));
        var button = new Button {
            Theme = Assert.IsType<ControlTheme>(resource), Height = 32,
            HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Left,
            VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Top,
            Content = new LucideIcon { Kind = LucideIconKind.X }
        };
        button.Classes.Add("VhilzCaptionClose");
        var window = new Window { Width = 300, Height = 200, Content = button, Foreground = Brushes.Black };
        window.Show();
        try {
            var surface = Assert.Single(button.GetVisualDescendants().OfType<AcrylicSurface>());
            var transition = Assert.Single(surface.Transitions!.OfType<ColorTransition>());
            Assert.Equal(TimeSpan.FromMilliseconds(400), transition.Duration);
            var point = button.TranslatePoint(new Point(20, 16), window)!.Value;
            foreach (var variant in new[] { ThemeVariant.Dark, ThemeVariant.Light }) {
                window.RequestedThemeVariant = variant;
                window.MouseMove(point);
                Assert.Equal(TimeSpan.FromMilliseconds(120), transition.Duration);
                Assert.IsType<SineEaseInOut>(transition.Easing);
                Assert.NotEqual(Assert.IsAssignableFrom<ISolidColorBrush>(button.Background).Color,
                    surface.SurfaceColor);
                await TickAfter(45);
                Assert.InRange(surface.SurfaceColor.A, (byte)1,
                    (byte)(Assert.IsAssignableFrom<ISolidColorBrush>(button.Background).Color.A - 1));
                var intermediate = surface.SurfaceColor;
                window.MouseMove(new Point(250, 150));
                Assert.Equal(TimeSpan.FromMilliseconds(400), transition.Duration);
                Assert.IsType<CubicEaseOut>(transition.Easing);
                // 再次插值会产生至多几个色阶的 sRGB 舍入差，不应跳回透明或目标色。
                Assert.InRange(Math.Abs(intermediate.A - surface.SurfaceColor.A), 0, 2);
                Assert.InRange(Math.Abs(intermediate.R - surface.SurfaceColor.R), 0, 2);
                Assert.InRange(Math.Abs(intermediate.G - surface.SurfaceColor.G), 0, 2);
                Assert.InRange(Math.Abs(intermediate.B - surface.SurfaceColor.B), 0, 2);
                await TickAfter(480);
                Assert.Equal(Colors.Transparent, surface.SurfaceColor);

                window.MouseMove(point);
                await TickAfter(340);
                Assert.Equal(Assert.IsAssignableFrom<ISolidColorBrush>(button.Background).Color, surface.SurfaceColor);
                Assert.Equal(Colors.White, Assert.IsAssignableFrom<ISolidColorBrush>(button.Foreground).Color);
                window.MouseMove(new Point(250, 150));
                await TickAfter(260);
                // 超过普通悬停时长后红色仍在退场；再次移入应接续当前颜色。
                Assert.InRange(surface.SurfaceColor.A, (byte)1, (byte)40);
                var fading = surface.SurfaceColor;
                window.MouseMove(point);
                Assert.Same(transition, Assert.Single(surface.Transitions!.OfType<ColorTransition>()));
                Assert.Equal(TimeSpan.FromMilliseconds(120), transition.Duration);
                Assert.IsType<SineEaseInOut>(transition.Easing);
                Assert.InRange(Math.Abs(fading.A - surface.SurfaceColor.A), 0, 2);
                Assert.InRange(Math.Abs(fading.R - surface.SurfaceColor.R), 0, 2);
                Assert.InRange(Math.Abs(fading.G - surface.SurfaceColor.G), 0, 2);
                Assert.InRange(Math.Abs(fading.B - surface.SurfaceColor.B), 0, 2);
                await TickAfter(340);
                Assert.Equal(Assert.IsAssignableFrom<ISolidColorBrush>(button.Background).Color, surface.SurfaceColor);
                window.MouseDown(point, MouseButton.Left);
                Assert.Equal(TimeSpan.FromMilliseconds(80), transition.Duration);
                Assert.IsType<SineEaseInOut>(transition.Easing);
                await TickAfter(120);
                Assert.Equal(Assert.IsAssignableFrom<ISolidColorBrush>(button.Background).Color, surface.SurfaceColor);
                window.MouseUp(point, MouseButton.Left);
                Assert.Equal(TimeSpan.FromMilliseconds(120), transition.Duration);
                window.MouseMove(new Point(250, 150));
                await TickAfter(480);
                Assert.Equal(Colors.Transparent, surface.SurfaceColor);
                Assert.Equal(Colors.Black, Assert.IsAssignableFrom<ISolidColorBrush>(button.Foreground).Color);
            }

            // 应用在运行时覆盖新键后，已有背景 Transition 应继续更新，并按目标状态选择参数。
            window.Resources["Vhilz.CaptionButton.Motion.Color.Default.Duration"] = TimeSpan.FromMilliseconds(200);
            window.MouseMove(point);
            Assert.Equal(TimeSpan.FromMilliseconds(200), transition.Duration);
            window.Resources["Vhilz.CaptionButton.Motion.Color.Default.Duration"] = TimeSpan.FromMilliseconds(180);
            Assert.Equal(TimeSpan.FromMilliseconds(180), transition.Duration);
            window.Resources["Vhilz.CaptionButton.Motion.Background.ToPressed.Duration"] =
                TimeSpan.FromMilliseconds(60);
            window.MouseDown(point, MouseButton.Left);
            Assert.Equal(TimeSpan.FromMilliseconds(60), transition.Duration);
            window.MouseUp(point, MouseButton.Left);
            Assert.Equal(TimeSpan.FromMilliseconds(180), transition.Duration);
            window.Resources["Vhilz.CaptionButton.Close.Motion.Background.ToNormal.Duration"] =
                TimeSpan.FromMilliseconds(300);
            window.MouseMove(new Point(250, 150));
            Assert.Equal(TimeSpan.FromMilliseconds(300), transition.Duration);
            Assert.Same(transition, Assert.Single(surface.Transitions!.OfType<ColorTransition>()));
        }
        finally {
            window.Close();
        }
    }

    private static async Task TickAfter(int milliseconds) {
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        await Task.Delay(milliseconds);
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        global::Avalonia.Threading.Dispatcher.UIThread.RunJobs();
    }

    private static void AssertBrushResource(VhilzTheme theme, ThemeVariant variant, string key, IBrush? actual) {
        Assert.True(theme.TryGetResource(key, variant, out var expected));
        Assert.Equal(Assert.IsAssignableFrom<ISolidColorBrush>(expected).Color,
            Assert.IsAssignableFrom<ISolidColorBrush>(actual).Color);
    }
}
