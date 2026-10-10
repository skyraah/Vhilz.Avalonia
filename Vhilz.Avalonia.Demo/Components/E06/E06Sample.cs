using Avalonia.Controls;
using Avalonia.Layout;
using static Vhilz.Avalonia.Demo.Components.NativeSampleLayout;

namespace Vhilz.Avalonia.Demo.Components.E06;

public sealed class E06Sample : UserControl
{
    public E06Sample()
    {
        Content = Column(Text("GroupBox · 原生带标题分组"),
            new GroupBox { Header = "账户设置", Content = Column(Text("团队名称"), new TextBox { Text = "Acme Studio" }, new Button { Content = "保存设置" }) },
            new GroupBox { Header = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Children = { Text("权限"), new CheckBox { Content = "启用", IsChecked = true } } }, Content = Text("复杂标题直接使用 Header，不生成第二种卡片类型。") },
            new GroupBox { Header = "空内容" },
            new GroupBox { Header = "嵌套与长内容", Content = new GroupBox { Header = "内部分组", Content = Text(string.Join(" ", Enumerable.Repeat("内容可自然换行。", 12))) } });
    }

}
