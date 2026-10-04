using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Headless.XUnit;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Lucide.Avalonia;
using OverlayDialogHost = Ursa.Controls.OverlayDialogHost;
using VhilzOverlayDialogHost = Vhilz.Avalonia.Theme.Controls.OverlayDialogHost;
using Vhilz.Avalonia.Theme;
using Vhilz.Avalonia.Theme.Controls;
using Xunit;

namespace Vhilz.Avalonia.Tests;

public class VhilzWindowTests
{
    [AvaloniaFact]
    public void DerivedWindowUsesRegisteredThemeAndItsSetters()
    {
        var theme = new VhilzTheme();
        Assert.True(theme.TryGetResource("Vhilz.Window.Theme", null, out var resource));
        var implementation = Assert.IsType<ControlTheme>(resource);
        Assert.Null(implementation.BasedOn);
        implementation.Setters.Add(new Setter(VhilzWindow.TitleBarPaddingProperty, new Thickness(12)));
        implementation.Setters.Add(new Setter(VhilzWindow.TitleBarBackgroundProperty, Brushes.Red));

        var content = new Border();
        var window = new DerivedWindow { Content = content };
        window.Styles.Add(theme);
        window.Show();
        try
        {
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
            Assert.Single(window.GetLogicalChildren().OfType<VhilzOverlayDialogHost>());
            Assert.Equal(typeof(WindowDrawnDecorations), window.WindowDecorationsTheme!.TargetType);
            Assert.Null(window.WindowDecorationsTheme.BasedOn);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void WrappedTitleBarAndHostReuseUrsaPropertiesAndModalReporting()
    {
        var left = new Border();
        var center = new Border();
        var right = new Border();
        var window = new VhilzWindow
        {
            LeftContent = left,
            TitleBarContent = center,
            RightContent = right
        };
        window.Show();
        try
        {
            var titleBar = Assert.Single(window.GetVisualDescendants().OfType<TitleBar>());
            Assert.IsAssignableFrom<Ursa.Controls.TitleBar>(titleBar);
            Assert.Equal(typeof(TitleBar), titleBar.StyleKey);
            var presenters = titleBar.GetVisualDescendants().OfType<ContentPresenter>().ToArray();
            Assert.Contains(presenters, presenter => ReferenceEquals(presenter.Content, left));
            Assert.Contains(presenters, presenter => ReferenceEquals(presenter.Content, center));
            Assert.Contains(presenters, presenter => ReferenceEquals(presenter.Content, right));

            var replacement = new Border();
            window.TitleBarContent = replacement;
            Assert.Same(replacement, titleBar.CenterContent);
            window.IsTitleBarVisible = false;
            Assert.False(titleBar.IsVisible);

            var host = Assert.Single(window.GetLogicalChildren().OfType<VhilzOverlayDialogHost>());
            Assert.True(host.IsTopLevel);
            Assert.True(host.IsModalStatusReporter);
            host.IsInModalStatus = true;
            Assert.True(OverlayDialogHost.GetIsInModalStatus(window));
            host.IsInModalStatus = false;
            Assert.False(OverlayDialogHost.GetIsInModalStatus(window));
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void DecorationsUseOwnButtonThemesAndFrameworkPartContract()
    {
        var theme = new VhilzTheme();
        Assert.True(theme.TryGetResource("Vhilz.WindowDecorations.Theme", null, out var resource));
        var decorations = Assert.IsType<ControlTheme>(resource);
        var templateSetter = Assert.Single(decorations.Setters.OfType<Setter>(),
            setter => setter.Property == WindowDrawnDecorations.TemplateProperty);
        var template = Assert.IsAssignableFrom<IWindowDrawnDecorationsTemplate>(templateSetter.Value);
        var result = template.Build();
        Assert.NotNull(result.Result.Underlay);
        Assert.NotNull(result.Result.Overlay);
        Assert.NotNull(result.Result.FullscreenPopover);
        foreach (var part in new[]
                 {
                     "PART_MinimizeButton", "PART_MaximizeButton", "PART_FullScreenButton", "PART_CloseButton",
                     "PART_PopoverFullScreenButton", "PART_PopoverCloseButton"
                 })
        {
            var button = Assert.IsType<Button>(result.NameScope.Find(part));
            Assert.NotNull(button.Theme);
            Assert.Null(button.Theme.BasedOn);
        }
    }

    [AvaloniaFact]
    public void RestoreIconPreservesLucideScaleAndFollowsButtonForeground()
    {
        var theme = new VhilzTheme();
        Assert.True(theme.TryGetResource("Vhilz.WindowDecorations.Theme", null, out var resource));
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
        try
        {
            Assert.Equal(new Size(lucide.Size, lucide.Size), restore.Bounds.Size);
            Assert.Equal(1.5, restore.StrokeWidth);
            Assert.NotNull(restore.Data);
            Assert.Empty(restore.GetVisualChildren());

            foreach (var brush in new[] { Brushes.White, Brushes.Black })
            {
                button.Foreground = brush;
                Assert.Same(brush, restore.Foreground);
            }

            // 尺寸共享；还原框线使用独立描边 Token，并允许应用覆盖。
            window.Resources["Vhilz.CaptionButton.Icon.Size"] = 20d;
            window.Resources["Vhilz.CaptionButton.Icon.StrokeWidth"] = 1.5d;
            window.UpdateLayout();
            Assert.Equal(20, lucide.Size);
            Assert.Equal(new Size(lucide.Size, lucide.Size), restore.Bounds.Size);
            Assert.Equal(1.5, lucide.StrokeWidth);
            Assert.Equal(1.5, restore.StrokeWidth);
            window.Resources["Vhilz.CaptionButton.Icon.Restore.StrokeWidth"] = 2.25d;
            Assert.Equal(2.25, restore.StrokeWidth);
            Assert.Equal(1.5, lucide.StrokeWidth);
            var replacement = Geometry.Parse("M3 3h18v18H3z");
            window.Resources["Vhilz.CaptionButton.Icon.Restore.Geometry"] = replacement;
            Assert.Same(replacement, restore.Data);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void ReplacingOrClearingTemplateDetachesOldDialogHost()
    {
        var oldHost = new OverlayDialogHost();
        var newHost = new OverlayDialogHost();
        var window = new VhilzWindow { Template = CreateTemplate(oldHost) };
        window.Show();
        try
        {
            // 新宿主仍由 UrsaWindow 登记到窗口的逻辑子项。
            Assert.Contains(oldHost, window.GetLogicalChildren().OfType<OverlayDialogHost>());
            window.Template = CreateTemplate(newHost);
            window.ApplyTemplate();

            Assert.Equal(new[] { newHost }, window.GetLogicalChildren().OfType<OverlayDialogHost>());
            Assert.Null(((ILogical)oldHost).LogicalParent);

            window.Template = null;
            Assert.Empty(window.GetLogicalChildren().OfType<OverlayDialogHost>());

            window.Template = new FuncControlTemplate<VhilzWindow>((_, _) => new Panel());
            window.ApplyTemplate();
            Assert.Empty(window.GetLogicalChildren().OfType<OverlayDialogHost>());
            Assert.Null(((ILogical)newHost).LogicalParent);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void CloseUsesUrsaConfirmationAndRaisesClosingOnce()
    {
        var window = new ConfirmingWindow();
        var closingCount = 0;
        window.Closing += (_, _) => closingCount++;
        window.Show();
        try
        {
            window.Close();
            Assert.True(window.IsVisible);
            Assert.Equal(1, window.ConfirmationCount);
            var beforeAcceptedClose = closingCount;
            window.AllowClose = true;
            window.Close();
            Assert.False(window.IsVisible);
            Assert.Equal(2, window.ConfirmationCount);
            Assert.Equal(beforeAcceptedClose + 1, closingCount);
        }
        finally
        {
            window.AllowClose = true;
            window.Close();
        }
    }

    private static FuncControlTemplate<VhilzWindow> CreateTemplate(OverlayDialogHost host) =>
        new((_, scope) =>
        {
            scope.Register(VhilzWindow.PART_DialogHost, host);
            return new Panel { Children = { host } };
        });

    private sealed class DerivedWindow : VhilzWindow;

    private sealed class ConfirmingWindow : VhilzWindow
    {
        public bool AllowClose { get; set; }
        public int ConfirmationCount { get; private set; }

        protected override Task<bool> CanClose()
        {
            ConfirmationCount++;
            return Task.FromResult(AllowClose);
        }
    }
}
