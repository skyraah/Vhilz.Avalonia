using Vhilz.Avalonia.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Vhilz.Avalonia.Theme.Controls;
using Xunit;

namespace Vhilz.Avalonia.Tests;

public class ResourceDictionaryTests {
    [AvaloniaFact]
    public void ApplicationSemanticValueOverridesReachExistingButtonAndSurface() {
        var app = Application.Current!;
        var theme = new VhilzTheme();
        Assert.True(theme.TryGetResource(ResourceKeys.CaptionButton.Theme, null, out var resource));
        var button = new Button {
            Theme = Assert.IsType<ControlTheme>(resource),
            Height = 32
        };
        var window = new Window { Content = button };
        window.Show();
        try {
            var surface = Assert.Single(button.GetVisualDescendants().OfType<CaptionSurface>());
            foreach (var variant in new[] { ThemeVariant.Dark, ThemeVariant.Light }) {
                window.RequestedThemeVariant = variant;
                app.Resources[ResourceKeys.Radius.Small] = new CornerRadius(9);
                app.Resources[ResourceKeys.Duration.Fast] = TimeSpan.FromMilliseconds(60);
                Assert.Equal(new CornerRadius(9), button.CornerRadius);
                Assert.Equal(new CornerRadius(9), surface.CornerRadius);
                Assert.Equal(TimeSpan.FromMilliseconds(60), surface.PressedDuration);
                app.Resources.Remove(ResourceKeys.Radius.Small);
                app.Resources.Remove(ResourceKeys.Duration.Fast);
                Assert.Equal(new CornerRadius(5), button.CornerRadius);
                Assert.Equal(TimeSpan.FromMilliseconds(80), surface.PressedDuration);
            }

            // 单个控件使用 Setter/属性覆盖，不增加一套圆角别名。
            button.CornerRadius = new CornerRadius(7);
            app.Resources[ResourceKeys.Radius.Small] = new CornerRadius(11);
            Assert.Equal(new CornerRadius(7), surface.CornerRadius);
        }
        finally {
            app.Resources.Remove(ResourceKeys.Radius.Small);
            app.Resources.Remove(ResourceKeys.Duration.Fast);
            window.Close();
        }
    }

    [AvaloniaFact]
    public void StaticValueAliasKeepsLoadedValueWhileDirectDynamicReferenceTracksOverrides() {
        var dictionary = new ResourceInclude(new Uri("avares://Vhilz.Avalonia.Tests/")) {
            Source = new Uri("avares://Vhilz.Avalonia.Tests/ResourceAliasProbe.axaml")
        };
        var direct = new Border();
        var alias = new Border();
        var window = new Window { Content = new StackPanel { Children = { direct, alias } } };
        window.Resources.MergedDictionaries.Add(dictionary);
        direct.Bind(Border.CornerRadiusProperty, new DynamicResourceExtension("Probe.Radius"));
        alias.Bind(Border.CornerRadiusProperty, new DynamicResourceExtension("Probe.Alias"));
        window.Show();
        try {
            Assert.Equal(new CornerRadius(5), direct.CornerRadius);
            Assert.Equal(new CornerRadius(5), alias.CornerRadius);
            // StaticResource 保存加载时的值；动态引用别名也无法补回它与原键之间的联系。
            window.Resources["Probe.Radius"] = new CornerRadius(9);
            Assert.Equal(new CornerRadius(9), direct.CornerRadius);
            Assert.Equal(new CornerRadius(5), alias.CornerRadius);
            window.Resources.Remove("Probe.Radius");
            Assert.Equal(new CornerRadius(5), direct.CornerRadius);
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void ThemeDictionariesHaveMatchingKeysAndTypes() {
        var dark = LoadPalette("Dark");
        var light = LoadPalette("Light");
        Assert.NotEmpty(dark.Keys);
        Assert.Equal(dark.Keys.Cast<string>().Order(), light.Keys.Cast<string>().Order());
        foreach (var key in dark.Keys) {
            Assert.NotNull(dark[key]);
            Assert.NotNull(light[key]);
            Assert.Equal(dark[key]!.GetType(), light[key]!.GetType());
        }
    }

    [AvaloniaFact]
    public void SharedPaletteKeepsSimultaneousWindowVariantsAndSupportsApplicationReplacement() {
        var app = Application.Current!;
        var dark = new VhilzWindow { RequestedThemeVariant = ThemeVariant.Dark };
        var light = new VhilzWindow { RequestedThemeVariant = ThemeVariant.Light };
        const string sharedKey = ResourceKeys.Color.Surface;
        dark.Show();
        light.Show();
        try {
            AssertColor("#18181B", dark.Background);
            AssertColor("#FAFAFA", light.Background);
            AssertColor("#FAFAFA", dark.Foreground);
            AssertColor("#18181B", light.Foreground);
            var darkTitle = Assert.Single(dark.GetVisualDescendants().OfType<TitleBar>());
            var lightTitle = Assert.Single(light.GetVisualDescendants().OfType<TitleBar>());
            AssertColor("#18181B", darkTitle.Background);
            AssertColor("#FAFAFA", lightTitle.Background);

            // 共享颜色由主题资源宿主解析；应用级覆盖同时更新两种变体。
            app.Resources[sharedKey] = Colors.Navy;
            AssertColor("Navy", dark.Background);
            AssertColor("Navy", light.Background);
            AssertColor("Navy", darkTitle.Background);
            AssertColor("Navy", dark.TransparencyBackgroundFallback);

            // 控件画笔仍是局部入口，覆盖标题栏不影响同一窗口的正文表面。
            dark.Resources[ResourceKeys.Window.TitleBar.BackgroundBrush] = Brushes.Maroon;
            app.Resources[sharedKey] = Colors.Teal;
            Assert.Same(Brushes.Maroon, darkTitle.Background);
            AssertColor("Teal", dark.Background);
            AssertColor("Teal", lightTitle.Background);

            dark.Resources.Remove(ResourceKeys.Window.TitleBar.BackgroundBrush);
            AssertColor("Teal", darkTitle.Background);
            app.Resources.Remove(sharedKey);
            AssertColor("#18181B", dark.Background);
            AssertColor("#FAFAFA", light.Background);

            dark.RequestedThemeVariant = ThemeVariant.Light;
            light.RequestedThemeVariant = ThemeVariant.Dark;
            AssertColor("#FAFAFA", dark.Background);
            AssertColor("#18181B", light.Background);
        }
        finally {
            app.Resources.Remove(sharedKey);
            dark.Close();
            light.Close();
        }
    }

    private static ResourceDictionary LoadPalette(string variant) =>
        Assert.IsType<ResourceDictionary>(new ResourceInclude(new Uri("avares://Vhilz.Avalonia/")) {
            Source = new Uri($"avares://Vhilz.Avalonia/Themes/Resources/{variant}.axaml")
        }.Loaded);

    private static void AssertColor(string expected, IBrush? actual) =>
        Assert.Equal(Color.Parse(expected), Assert.IsAssignableFrom<ISolidColorBrush>(actual).Color);
}
