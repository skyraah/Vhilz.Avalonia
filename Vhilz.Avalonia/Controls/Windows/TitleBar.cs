namespace Vhilz.Avalonia.Theme.Controls;

/// <summary>
/// 使用 Vhilz 主题的 Ursa 标题栏；内容槽与属性直接继承基类。
/// </summary>
public class TitleBar : Ursa.Controls.TitleBar
{
    /// <inheritdoc />
    protected override Type StyleKeyOverride => typeof(TitleBar);
}
