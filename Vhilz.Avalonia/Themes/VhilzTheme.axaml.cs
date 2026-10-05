using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Vhilz.Avalonia.Theme;

/// <summary>
/// 加载 Vhilz 自有的明暗资源、通用资源与控件主题。
/// </summary>
public partial class VhilzTheme : Styles
{
    /// <summary>
    /// 初始化主题资源与样式入口。
    /// </summary>
    public VhilzTheme()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
