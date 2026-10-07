using Vhilz.Avalonia.Theme;
using System.Reflection;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Vhilz.Avalonia.Theme.Controls;
using Vhilz.Avalonia.Theme.Platforms;
using Xunit;

namespace Vhilz.Avalonia.Tests;

public class FullscreenWindowTests {
    [AvaloniaFact]
    public void HeadlessDoesNotAttachNativeAnimation() {
        var window = CreateWindow();
        window.Show();
        try {
            Assert.Null(FullscreenTransition.TryAttach(window));
            window.WindowState = WindowState.FullScreen;
            Assert.Equal(WindowState.FullScreen, window.WindowState);
            window.WindowState = WindowState.Normal;
            Assert.Equal(WindowState.Normal, window.WindowState);
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void EnteringFullscreenRemovesClientTitleBarDragArea(bool light) {
        var window = CreateWindow();
        window.RequestedThemeVariant = light ? ThemeVariant.Light : ThemeVariant.Dark;
        window.Show();
        try {
            var decorations = ApplyDecorations(window);
            Click(FindButton(decorations.Content!.Overlay!, "PART_FullScreenButton"));
            Assert.Equal(WindowState.FullScreen, window.WindowState);
            window.UpdateLayout();
            var titleBar = Assert.Single(window.GetVisualDescendants().OfType<TitleBar>());
            Assert.False(titleBar.IsEffectivelyVisible);
            var content = (Control)window.Content!;
            Assert.Equal(0, content.TranslatePoint(default, window)!.Value.Y);
            Assert.True(window.IsTitleBarVisible);
            window.WindowState = WindowState.Normal;
            window.UpdateLayout();
            Assert.True(titleBar.IsEffectivelyVisible);
            Assert.True(content.TranslatePoint(default, window)!.Value.Y > 0);

            window.IsTitleBarVisible = false;
            window.WindowState = WindowState.FullScreen;
            window.WindowState = WindowState.Normal;
            Assert.False(titleBar.IsEffectivelyVisible);
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void TopEdgeRevealsUsableExitButton(bool light) {
        var window = CreateWindow();
        window.RequestedThemeVariant = light ? ThemeVariant.Light : ThemeVariant.Dark;
        window.Show();
        try {
            // 此用例检查最终几何；动画插值另有回归覆盖。
            FindFullscreenInset(window).Transitions = null;
            var decorations = ApplyDecorations(window);
            Click(FindButton(decorations.Content!.Overlay!, "PART_FullScreenButton"));
            Assert.Equal(WindowState.FullScreen, window.WindowState);
            decorations = ApplyDecorations(window);
            window.MouseMove(new Point(300, 200));
            window.MouseMove(new Point(300, 0));
            window.UpdateLayout();
            var exit = FindButton(decorations.Content!.FullscreenPopover!, "PART_PopoverFullScreenButton");
            Assert.True(exit.IsEffectivelyVisible);
            Assert.True(exit.Bounds.Height > 0);
            var popover = decorations.Content.FullscreenPopover!;
            Assert.Equal(new Thickness(6, 3), popover.Margin);
            Assert.Equal(new Point(6, 3), popover.TranslatePoint(default, window)!.Value);
            Assert.Equal(window.Bounds.Width - 12, popover.Bounds.Width);
            var content = (Control)window.Content!;
            Assert.Equal(popover.Bounds.Height + 6, content.TranslatePoint(default, window)!.Value.Y);
            window.Resources[ResourceKeys.Window.TitleBar.Height] = 48d;
            window.UpdateLayout();
            Assert.Equal(48, popover.Bounds.Height);
            Assert.Equal(window.Bounds.Width - 12, popover.Bounds.Width);
            Assert.Equal(54, content.TranslatePoint(default, window)!.Value.Y);
            window.IsFullScreenButtonVisible = false;
            Assert.True(exit.IsEffectivelyVisible);
            Assert.NotEqual(WindowDecorationsElementRole.TitleBar,
                WindowDecorationProperties.GetElementRole(decorations.Content.FullscreenPopover!));
            window.MouseMove(new Point(300, 200));
            Assert.False(exit.IsEffectivelyVisible);
            window.UpdateLayout();
            Assert.Equal(0, content.TranslatePoint(default, window)!.Value.Y);
            window.MouseMove(new Point(300, 0));
            window.UpdateLayout();
            Assert.True(exit.IsEffectivelyVisible);
            Click(exit);
            Assert.Equal(WindowState.Normal, window.WindowState);
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void FullscreenRevealSlidesFromTopAndMovesContentInLockstep(bool light) {
        var window = CreateWindow();
        window.RequestedThemeVariant = light ? ThemeVariant.Light : ThemeVariant.Dark;
        window.Show();
        try {
            var inset = FindFullscreenInset(window);
            var transition = Assert.Single(inset.Transitions!.OfType<DoubleTransition>());
            Assert.Equal(TimeSpan.FromMilliseconds(160), transition.Duration);
            // 使用框架时钟的精确脉冲验证生产 Transition，避免依赖机器执行速度。
            var clock = new TestAnimationClock(inset);
            void Advance(int milliseconds) {
                clock.AdvanceTo(milliseconds);
                window.UpdateLayout();
            }

            window.WindowState = WindowState.FullScreen;
            var decorations = ApplyDecorations(window);
            var popover = decorations.Content!.FullscreenPopover!;
            var content = (Control)window.Content!;
            double Extent() => popover.Bounds.Height + popover.Margin.Top + popover.Margin.Bottom;
            double ContentTop() => content.TranslatePoint(default, window)!.Value.Y;

            void AssertSynchronized() => Assert.Equal(ContentTop(),
                popover.TranslatePoint(default, window)!.Value.Y + popover.Bounds.Height + popover.Margin.Bottom);

            window.MouseMove(new Point(300, 0));
            window.UpdateLayout();
            Advance(0);
            Assert.Equal(-popover.Bounds.Height - popover.Margin.Bottom, popover.TranslatePoint(default, window)!.Value.Y);
            Assert.Equal(0, ContentTop());
            window.MouseMove(new Point(301, 1));
            Advance(80);
            Assert.InRange(ContentTop(), 1, Extent() - 1);
            AssertSynchronized();
            Advance(200);
            Assert.Equal(Extent(), ContentTop());
            Assert.Equal(popover.Margin.Top, popover.TranslatePoint(default, window)!.Value.Y);

            window.MouseMove(new Point(300, 200));
            Advance(200);
            Assert.True(popover.IsEffectivelyVisible);
            Assert.Equal(Extent(), ContentTop());
            Advance(240);
            var retracting = ContentTop();
            Assert.InRange(retracting, 1, Extent() - 1);
            AssertSynchronized();
            // 收回途中回到顶栏范围，以当前高度反向，不能先跳到零或满高。
            window.MouseMove(new Point(300, 10));
            Advance(240);
            Assert.Equal(retracting, ContentTop());
            Advance(320);
            Assert.True(ContentTop() > retracting);
            AssertSynchronized();
            Advance(440);
            Assert.Equal(Extent(), ContentTop());
            window.MouseMove(new Point(300, 200));
            Advance(440);
            Advance(640);
            Assert.Equal(0, ContentTop());
            Assert.False(popover.IsEffectivelyVisible);

            for (var cycle = 0; cycle < 5; cycle++) {
                var start = 700 + cycle * 500;
                window.MouseMove(new Point(300, 0));
                window.UpdateLayout();
                Advance(start);
                var previous = 0d;
                for (var step = 1; step <= 10; step++) {
                    window.MouseMove(new Point(300 + step, step));
                    Advance(start + step * 20);
                    Assert.True(popover.IsEffectivelyVisible);
                    Assert.True(ContentTop() >= previous);
                    if (cycle == 0 && step == 3) {
                        window.Resources[ResourceKeys.Window.TitleBar.Height] = 48d;
                        window.UpdateLayout();
                    }

                    Assert.Equal(Extent(), inset.GetBaseValue(global::Avalonia.Layout.Layoutable.HeightProperty).Value);
                    AssertSynchronized();
                    previous = ContentTop();
                }

                window.MouseMove(new Point(300, 200));
                Advance(start + 200);
                Advance(start + 400);
                Assert.Equal(0, ContentTop());
                Assert.False(popover.IsEffectivelyVisible);
            }

            // 首帧前移出，以及收回途中退出全屏，都不能留下空白。
            window.MouseMove(new Point(300, 0));
            window.UpdateLayout();
            window.MouseMove(new Point(300, 200));
            Advance(3400);
            Assert.Equal(0, ContentTop());
            window.MouseMove(new Point(300, 0));
            Advance(3400);
            Advance(3600);
            window.MouseMove(new Point(300, 200));
            window.WindowState = WindowState.Normal;
            window.UpdateLayout();
            Assert.Equal(0, inset.Height);
            Assert.False(inset.IsAnimating(global::Avalonia.Layout.Layoutable.HeightProperty));
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void EscapeExitsFullscreenWithoutDrawnDecorations() {
        var window = CreateWindow();
        window.Show();
        try {
            window.WindowState = WindowState.FullScreen;
            window.KeyPress(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null);
            Assert.Equal(WindowState.Normal, window.WindowState);
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void HostExitInsideCaptionButtonsKeepsTitleBarOpenButOutsideRetracts(bool light) {
        var window = CreateWindow();
        window.RequestedThemeVariant = light ? ThemeVariant.Light : ThemeVariant.Dark;
        window.Show();
        try {
            var inset = FindFullscreenInset(window);
            inset.Transitions = null;
            window.WindowState = WindowState.FullScreen;
            var popover = ApplyDecorations(window).Content!.FullscreenPopover!;
            var host = Assert.IsAssignableFrom<InputElement>(window.GetVisualParent());
            var pointer = new global::Avalonia.Input.Pointer(1, PointerType.Mouse, true);

            void ExitAt(Point position) {
                // 重放 Win32 客户区/非客户区切换时的宿主退出通知，坐标仍可能位于顶栏内。
                host.RaiseEvent(new PointerEventArgs(InputElement.PointerExitedEvent, host, pointer, host,
                    position, 0, new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.Other),
                    KeyModifiers.None));
                window.UpdateLayout();
            }

            window.MouseMove(new Point(300, 0));
            window.UpdateLayout();
            var extent = popover.Bounds.Height + popover.Margin.Top + popover.Margin.Bottom;
            foreach (var name in new[] { "PART_PopoverFullScreenButton", "PART_PopoverCloseButton" }) {
                var button = FindButton(popover, name);
                ExitAt(button.TranslatePoint(new Point(button.Bounds.Width / 2, button.Bounds.Height / 2), host)!
                    .Value);
                Assert.True(popover.IsEffectivelyVisible);
                Assert.Equal(extent, inset.Height);
            }

            foreach (var outside in new[] {
                         new Point(-1, 16), new Point(window.Bounds.Width + 1, 16),
                         new Point(300, extent + 1), new Point(-1, -1)
                     }) {
                ExitAt(outside);
                Assert.Equal(0, inset.Height);
                Assert.False(popover.IsEffectivelyVisible);
                window.MouseMove(new Point(300, 0));
                window.UpdateLayout();
            }
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void ContentCanConsumeEscapeBeforeWindowExitsFullscreen() {
        var content = new Button();
        var window = CreateWindow();
        window.Content = content;
        EventHandler<KeyEventArgs> consumeEscape = (_, e) => e.Handled = e.Key == Key.Escape;
        content.KeyDown += consumeEscape;
        window.Show();
        try {
            content.Focus();
            window.WindowState = WindowState.FullScreen;
            window.KeyPress(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null);
            window.KeyRelease(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null);
            Assert.Equal(WindowState.FullScreen, window.WindowState);
            content.KeyDown -= consumeEscape;
            window.KeyPress(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null);
            Assert.Equal(WindowState.Normal, window.WindowState);
        }
        finally {
            window.Close();
        }
    }

    private static VhilzWindow CreateWindow() => new() {
        Width = 600, Height = 400, IsFullScreenButtonVisible = true,
        ExtendClientAreaToDecorationsHint = true,
        Content = new Border()
    };

    private static void Click(Button button) => button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

    private static Border FindFullscreenInset(VhilzWindow window) =>
        Assert.Single(window.GetVisualDescendants().OfType<Border>(),
            border => border.Name == VhilzWindow.PartFullscreenTitleBarInset);

    private static Button FindButton(Control root, string name) =>
        Assert.Single(root.GetVisualDescendants().OfType<Button>(), button => button.Name == name);

    private static WindowDrawnDecorations ApplyDecorations(VhilzWindow window) {
        // Headless 不提供平台装饰，使用框架入口挂载生产模板；全屏时仅保留浮层。
        var host = window.GetVisualParent()!;
        var update = host.GetType()
            .GetMethod("UpdateDrawnDecorations", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var partsType = Nullable.GetUnderlyingType(update.GetParameters()[0].ParameterType)!;
        update.Invoke(host, new[] {
            Enum.Parse(partsType, window.WindowState == WindowState.FullScreen ? "None" : "TitleBar, Border"),
            window.WindowState, window.WindowDecorationsTheme
        });
        window.UpdateLayout();
        return Assert.Single(((StyledElement)host).GetLogicalChildren().OfType<WindowDrawnDecorations>());
    }
}
