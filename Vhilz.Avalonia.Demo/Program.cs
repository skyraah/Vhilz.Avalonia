using Avalonia;
using System;

namespace Vhilz.Avalonia.Demo;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .With(CreateX11Options())
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();

    private static X11PlatformOptions CreateX11Options()
    {
        // XWayland 下的平铺合成器不提供标题栏，Demo 由 Avalonia 绘制装饰。
        // Avalonia 12.1.3 将 X11 自绘装饰开关标记为实验 API，仅在此处启用。
#pragma warning disable AVALONIA_X11_CSD
        return new X11PlatformOptions { EnableDrawnDecorations = true };
#pragma warning restore AVALONIA_X11_CSD
    }
}
