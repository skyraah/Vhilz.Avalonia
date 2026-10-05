using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Lucide.Avalonia;
using Vhilz.Avalonia.Theme;
using Vhilz.Avalonia.Theme.Controls;
using Xunit;

namespace Vhilz.Avalonia.Tests;

public class VhilzWindowTests {
    [AvaloniaFact]
    public void DerivedWindowUsesRegisteredThemeAndItsSetters() {
        var theme = new VhilzTheme();
        Assert.True(theme.TryGetResource(ResourceKeys.Window.Theme, null, out var resource));
        var implementation = Assert.IsType<ControlTheme>(resource);
        Assert.Null(implementation.BasedOn);
        implementation.Setters.Add(new Setter(VhilzWindow.TitleBarPaddingProperty, new Thickness(12)));
        Assert.Single(implementation.Setters.OfType<Setter>(),
            setter => setter.Property == VhilzWindow.TitleBarBackgroundProperty).Value = Brushes.Red;

        var content = new Border();
        var window = new DerivedWindow { Content = content };
        window.Styles.Add(theme);
        window.Show();
        try {
            Assert.Equal(typeof(VhilzWindow), window.StyleKey);
            Assert.NotNull(window.Template);
            Assert.Contains(window.GetVisualDescendants().OfType<ContentPresenter>(),
                presenter => ReferenceEquals(presenter.Content, content));
            Assert.Equal(new Thickness(12), window.TitleBarPadding);
            Assert.Same(Brushes.Red, window.TitleBarBackground);
            var titleBar = Assert.Single(window.GetVisualDescendants().OfType<TitleBar>());
            Assert.NotNull(titleBar.Template);
            Assert.Equal(new Thickness(12), titleBar.Padding);
            Assert.Same(Brushes.Red, titleBar.Background);
            Assert.Equal(typeof(WindowDrawnDecorations), window.WindowDecorationsTheme!.TargetType);
            Assert.Null(window.WindowDecorationsTheme.BasedOn);
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void TitleBarContentSlotsTrackWindowProperties() {
        var left = new Border();
        var center = new Border();
        var right = new Border();
        var window = new VhilzWindow {
            LeftContent = left,
            CenterContent = center,
            RightContent = right
        };
        window.Show();
        try {
            var titleBar = Assert.Single(window.GetVisualDescendants().OfType<TitleBar>());
            Assert.Equal(typeof(TitleBar), titleBar.StyleKey);
            var presenters = titleBar.GetVisualDescendants().OfType<ContentPresenter>().ToArray();
            Assert.Contains(presenters, presenter => ReferenceEquals(presenter.Content, left));
            Assert.Contains(presenters, presenter => ReferenceEquals(presenter.Content, center));
            Assert.Contains(presenters, presenter => ReferenceEquals(presenter.Content, right));

            var replacement = new Border();
            window.CenterContent = replacement;
            Assert.Same(replacement, titleBar.CenterContent);
            window.IsTitleBarVisible = false;
            Assert.False(titleBar.IsVisible);

            window.IsTitleBarVisible = true;
            Assert.True(titleBar.IsVisible);
            window.TitleBarMargin = new Thickness(4, 2, 8, 2);
            Assert.Equal(window.TitleBarMargin, titleBar.Margin);
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void TitleBarTracksWindowTitleAndPreservesContentSlots() {
        var left = new Border { Width = 24 };
        var center = new Border { Width = 60 };
        var right = new Border { Width = 40 };
        var window = new VhilzWindow {
            Width = 400,
            Title = "示例窗口",
            LeftContent = left,
            CenterContent = center,
            RightContent = right
        };
        window.Show();
        try {
            var titleBar = Assert.Single(window.GetVisualDescendants().OfType<TitleBar>());
            var title = Assert.Single(titleBar.GetVisualDescendants().OfType<TextBlock>(),
                textBlock => textBlock.Name == "PART_WindowTitle");
            Assert.Equal("示例窗口", title.Text);
            Assert.False(title.IsHitTestVisible);
            Assert.Equal(WindowDecorationsElementRole.TitleBar,
                WindowDecorationProperties.GetElementRole((Control)title.Parent!));
            Assert.Contains(titleBar.GetVisualDescendants().OfType<ContentPresenter>(),
                presenter => ReferenceEquals(presenter.Content, left));

            window.Title = "更新后的标题";
            Assert.Contains(titleBar.GetVisualDescendants().OfType<TextBlock>(),
                textBlock => textBlock.Text == "更新后的标题");

            foreach (var variant in new[] { ThemeVariant.Dark, ThemeVariant.Light }) {
                window.RequestedThemeVariant = variant;
                titleBar.Classes.Remove("VhilzInactive");
                Assert.Equal(titleBar.Foreground, title.Foreground);
                titleBar.Classes.Add("VhilzInactive");
                Assert.Equal(titleBar.Foreground, title.Foreground);
            }

            window.Title = new string('长', 200);
            window.UpdateLayout();
            Assert.Equal(TextTrimming.CharacterEllipsis, title.TextTrimming);
            Assert.True(title.Bounds.Width > 0);
            Assert.True(title.Bounds.Width < titleBar.Bounds.Width);
            Assert.Equal(24, left.Bounds.Width);
            Assert.Equal(60, center.Bounds.Width);
            Assert.Equal(40, right.Bounds.Width);
            window.Title = string.Empty;
            Assert.False(title.IsVisible);
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void DecorationsUseOwnButtonThemesAndFrameworkPartContract() {
        var theme = new VhilzTheme();
        Assert.True(theme.TryGetResource(ResourceKeys.WindowDecorations.Theme, null, out var resource));
        var decorations = Assert.IsType<ControlTheme>(resource);
        var templateSetter = Assert.Single(decorations.Setters.OfType<Setter>(),
            setter => setter.Property == WindowDrawnDecorations.TemplateProperty);
        var template = Assert.IsAssignableFrom<IWindowDrawnDecorationsTemplate>(templateSetter.Value);
        var result = template.Build();
        Assert.NotNull(result.Result.Underlay);
        Assert.NotNull(result.Result.Overlay);
        Assert.NotNull(result.Result.FullscreenPopover);
        foreach (var part in new[] {
                     "PART_MinimizeButton", "PART_MaximizeButton", "PART_FullScreenButton", "PART_CloseButton",
                     "PART_PopoverFullScreenButton", "PART_PopoverCloseButton"
                 }) {
            var button = Assert.IsType<Button>(result.NameScope.Find(part));
            Assert.NotNull(button.Theme);
            Assert.Null(button.Theme.BasedOn);
        }
    }

    [AvaloniaFact]
    public void RestoreIconPreservesLucideScaleAndFollowsButtonForeground() {
        var theme = new VhilzTheme();
        Assert.True(theme.TryGetResource(ResourceKeys.WindowDecorations.Theme, null, out var resource));
        var decorations = Assert.IsType<ControlTheme>(resource);
        var templateSetter = Assert.Single(decorations.Setters.OfType<Setter>(),
            setter => setter.Property == WindowDrawnDecorations.TemplateProperty);
        var template = Assert.IsAssignableFrom<IWindowDrawnDecorationsTemplate>(templateSetter.Value);
        var result = template.Build();
        var button = Assert.IsType<Button>(result.NameScope.Find("PART_MaximizeButton"));
        var restore = Assert.IsType<CaptionGeometryIcon>(result.NameScope.Find("PartRestoreIcon"));
        var minimizeButton = Assert.IsType<Button>(result.NameScope.Find("PART_MinimizeButton"));
        var lucide = Assert.IsType<LucideIcon>(minimizeButton.Content);
        // 单独承载装饰模板，避免依赖 Headless 平台是否提供原生窗口装饰。
        var window = new Window { Content = result.Result.Overlay };
        restore.IsVisible = true;
        window.Show();
        try {
            Assert.Equal(new Size(lucide.Size, lucide.Size), restore.Bounds.Size);
            Assert.Equal(1.5, restore.StrokeWidth);
            Assert.NotNull(restore.Data);
            Assert.Empty(restore.GetVisualChildren());

            foreach (var brush in new[] { Brushes.White, Brushes.Black }) {
                button.Foreground = brush;
                Assert.Same(brush, restore.Foreground);
            }

            // 尺寸共享；还原框线使用独立描边 Token，并允许应用覆盖。
            window.Resources[ResourceKeys.CaptionButton.Icon.Size] = 20d;
            window.Resources[ResourceKeys.CaptionButton.Icon.StrokeWidth] = 1.5d;
            window.UpdateLayout();
            Assert.Equal(20, lucide.Size);
            Assert.Equal(new Size(lucide.Size, lucide.Size), restore.Bounds.Size);
            Assert.Equal(1.5, lucide.StrokeWidth);
            Assert.Equal(1.5, restore.StrokeWidth);
            window.Resources[ResourceKeys.CaptionButton.Icon.Restore.StrokeWidth] = 2.25d;
            Assert.Equal(2.25, restore.StrokeWidth);
            Assert.Equal(1.5, lucide.StrokeWidth);
            var replacement = Geometry.Parse("M3 3h18v18H3z");
            window.Resources[ResourceKeys.CaptionButton.Icon.Restore.Geometry] = replacement;
            Assert.Same(replacement, restore.Data);
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void ReplacingTemplatePreservesTitleBarContentAndBindings() {
        var content = new TextBlock();
        var window = new VhilzWindow { RightContent = content, DataContext = "标题栏数据" };
        window.Show();
        try {
            var originalTemplate = window.Template;
            var oldTitleBar = Assert.Single(window.GetVisualDescendants().OfType<TitleBar>());
            Assert.Equal(window.DataContext, content.DataContext);
            window.Template = new FuncControlTemplate<VhilzWindow>((_, _) => new Panel());
            window.ApplyTemplate();
            Assert.Empty(window.GetVisualDescendants().OfType<TitleBar>());
            window.Template = originalTemplate;
            window.ApplyTemplate();
            window.UpdateLayout();
            var newTitleBar = Assert.Single(window.GetVisualDescendants().OfType<TitleBar>());
            Assert.NotSame(oldTitleBar, newTitleBar);
            Assert.Same(content, newTitleBar.RightContent);
            window.DataContext = "更新数据";
            Assert.Equal(window.DataContext, content.DataContext);
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void ClosingCanCancelAndRaisesOncePerRequest() {
        var window = new VhilzWindow();
        var closingCount = 0;
        var allowClose = false;
        window.Closing += (_, e) => {
            closingCount++;
            e.Cancel = !allowClose;
        };
        window.Show();
        try {
            window.Close();
            Assert.True(window.IsVisible);
            Assert.Equal(1, closingCount);
            allowClose = true;
            window.Close();
            Assert.False(window.IsVisible);
            Assert.Equal(2, closingCount);
        }
        finally {
            allowClose = true;
            window.Close();
        }
    }

    [AvaloniaFact]
    public async Task ModalWindowReturnsResultAndReleasesOwner() {
        var owner = new VhilzWindow();
        var dialog = new VhilzWindow();
        owner.Show();
        try {
            var result = dialog.ShowDialog<bool>(owner);
            Assert.Same(owner, dialog.Owner);
            Assert.False(result.IsCompleted);
            dialog.Close(true);
            Assert.True(await result);
            Assert.DoesNotContain(dialog, owner.OwnedWindows);
            Assert.True(owner.IsVisible);
        }
        finally {
            dialog.Close();
            owner.Close();
        }
    }

    [AvaloniaFact]
    public void TitleBarReservesActualCaptionWidthAndKeepsApplicationMargin() {
        var window = new VhilzWindow {
            Width = 600, Height = 400, TitleBarMargin = new Thickness(3, 1, 5, 1),
            RightContent = new TextBlock { Text = "右侧内容" }
        };
        window.Show();
        try {
            // Headless 不生成平台装饰，通过框架入口装配真实装饰与按钮行为。
            var host = window.GetVisualParent()!;
            var update = host.GetType().GetMethod("UpdateDrawnDecorations",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
            var partsType = Nullable.GetUnderlyingType(update.GetParameters()[0].ParameterType)!;
            update.Invoke(host, new[] {
                Enum.Parse(partsType, "TitleBar, Border"), WindowState.Normal, window.WindowDecorationsTheme
            });
            typeof(Window).GetProperty(nameof(Window.IsExtendedIntoWindowDecorations))!.SetValue(window, true);
            window.UpdateLayout();

            var decorations = Assert.Single(((StyledElement)host).GetLogicalChildren().OfType<WindowDrawnDecorations>());
            var buttons = Assert.IsType<StackPanel>(decorations.Content!.Overlay);
            var inset = Assert.Single(window.GetVisualDescendants().OfType<Border>(), b => b.Name == VhilzWindow.PartTitleBarInset);
            var titleBar = Assert.Single(window.GetVisualDescendants().OfType<TitleBar>());
            Assert.True(inset.Padding.Right > 0);
            Assert.Equal(buttons.Bounds.Width + buttons.Margin.Left + buttons.Margin.Right, inset.Padding.Right);
            Assert.Equal(window.TitleBarMargin, titleBar.Margin);

            var previousWidth = inset.Padding.Right;
            window.IsMinimizeButtonVisible = false;
            window.UpdateLayout();
            Assert.True(inset.Padding.Right < previousWidth);
            Assert.Equal(window.TitleBarMargin, titleBar.Margin);

            window.WindowState = WindowState.FullScreen;
            window.UpdateLayout();
            Assert.Equal(default, inset.Padding);
        }
        finally {
            window.Close();
        }
    }

    private sealed class DerivedWindow : VhilzWindow;
}
