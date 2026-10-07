using Avalonia.Controls;
using Vhilz.Avalonia.Theme.Platforms.Windows;

namespace Vhilz.Avalonia.Theme.Platforms;

internal static class FullscreenTransition {
    // 平台选择集中于此；没有补充动画的平台继续使用 Avalonia 的原生切换。
    internal static IFullscreenTransition? TryAttach(Window window) =>
        OperatingSystem.IsWindows() ? WindowsFullscreenTransition.TryAttach(window) : null;
}
