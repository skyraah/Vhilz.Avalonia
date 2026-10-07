using Avalonia;

namespace Vhilz.Avalonia.Demo.Platforms.X11;

internal static class X11Options {
    internal static X11PlatformOptions Create() {
        // XWayland 下的平铺合成器不提供标题栏，Demo 由 Avalonia 绘制装饰。
        // Avalonia 12.1.3 将 X11 自绘装饰开关标记为实验 API，仅在此处启用。
#pragma warning disable AVALONIA_X11_CSD
        return new X11PlatformOptions { EnableDrawnDecorations = true };
#pragma warning restore AVALONIA_X11_CSD
    }
}
