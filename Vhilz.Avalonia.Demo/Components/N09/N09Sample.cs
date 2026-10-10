using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace Vhilz.Avalonia.Demo.Components.N09;

public sealed class N09Sample : UserControl {
    public N09Sample() {
        var panel = new StackPanel { Spacing = 16, MaxWidth = 720 };
        panel.Children.Add(new TextBlock { Text = "文本输入", FontSize = 20, FontWeight = FontWeight.SemiBold });
        panel.Children.Add(new TextBox { PlaceholderText = "输入项目名称 / Project name" });
        panel.Children.Add(new TextBox { Text = "选中并复制文本，可尝试中文输入、撤销和重做。" });
        var clear = new TextBox { Text = "获得焦点后显示清除按钮" };
        clear.Classes.Add("clearButton");
        panel.Children.Add(clear);
        var password = new TextBox { Text = "vhilz-password", PasswordChar = '●' };
        password.Classes.Add("revealPasswordButton");
        panel.Children.Add(password);
        panel.Children.Add(new TextBox { PlaceholderText = "多行说明", AcceptsReturn = true, TextWrapping = TextWrapping.Wrap, MinHeight = 100 });
        panel.Children.Add(new TextBox { Text = "此内容只读，仍然允许选择与复制", IsReadOnly = true });
        panel.Children.Add(new TextBox { Text = "禁用输入", IsEnabled = false });
        panel.Children.Add(new MaskedTextBox { Mask = "0000-00-00", PlaceholderText = "日期掩码：2026-10-09" });
        var invalid = new TextBox { Text = "", PlaceholderText = "必填名称" };
        void Validate() => DataValidationErrors.SetErrors(invalid, string.IsNullOrWhiteSpace(invalid.Text) ? new[] { "名称不能为空" } : null);
        invalid.TextChanged += (_, _) => Validate();
        Validate();
        panel.Children.Add(invalid);
        panel.Children.Add(new TextBlock { Text = "填写最后一项可清除校验错误。Tab 可观察键盘焦点。", TextWrapping = TextWrapping.Wrap });
        Content = panel;
    }
}
