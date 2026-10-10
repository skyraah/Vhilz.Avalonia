using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace Vhilz.Avalonia.Demo.Components.N02;

public sealed class N02Sample : UserControl {
    public N02Sample() {
        var panel = new StackPanel { Spacing = 16, MaxWidth = 720 };
        var title = new TextBlock { Text = "项目排版 · Typography" };
        title.Classes.Add("VhilzTextTitle"); panel.Children.Add(title);
        panel.Children.Add(new TextBlock { Text = "正文用于呈现可以直接理解的信息，保留英文与中文混排 / Body text.", TextWrapping = TextWrapping.Wrap });
        var secondary = new TextBlock { Text = "次级说明 / Secondary text" }; secondary.Classes.Add("VhilzTextSecondary"); panel.Children.Add(secondary);
        var caption = new TextBlock { Text = "更新时间：刚刚 / Updated now" }; caption.Classes.Add("VhilzTextCaption"); panel.Children.Add(caption);
        panel.Children.Add(new SelectableTextBlock { Text = "拖动选择这段文字，然后使用 Ctrl+C 或右键复制。Select and copy this paragraph.", TextWrapping = TextWrapping.Wrap });
        panel.Children.Add(new TextBlock { Text = "这是用于观察换行与截断的长文本。This is a long line to inspect truncation in a constrained layout.", Width = 260, TextTrimming = TextTrimming.CharacterEllipsis });
        panel.Children.Add(new TextBlock { Text = "" });
        var input = new TextBox { PlaceholderText = "输入姓名" }; panel.Children.Add(new Label { Content = "_姓名 / Name", Target = input }); panel.Children.Add(input);
        panel.Children.Add(new Label { Content = "禁用标签", IsEnabled = false });
        Content = panel;
    }
}
