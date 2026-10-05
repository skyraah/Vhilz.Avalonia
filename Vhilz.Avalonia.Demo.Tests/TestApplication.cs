using Avalonia;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Vhilz.Avalonia.Theme;

[assembly: AvaloniaTestApplication(typeof(Vhilz.Avalonia.Demo.Tests.TestApplication))]

namespace Vhilz.Avalonia.Demo.Tests;

public class TestApplication : Application
{
    public override void Initialize()
    {
        Styles.Add(new VhilzTheme());
    }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<TestApplication>().UseHeadless(new AvaloniaHeadlessPlatformOptions());
}
