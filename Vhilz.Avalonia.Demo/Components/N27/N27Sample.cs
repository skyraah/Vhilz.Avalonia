using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Vhilz.Avalonia.Theme.Controls;
using static Vhilz.Avalonia.Demo.Components.NativeSampleLayout;

namespace Vhilz.Avalonia.Demo.Components.N27;

public sealed class N27Sample : UserControl
{
    public N27Sample()
    {
        var result = Text("尚未打开对话框");
        var information = new Button { Content = "信息对话框" };
        var confirmation = new Button { Content = "确认归档" };
        var form = new Button { Content = "编辑个人资料" };
        information.Click += async (_, _) => await Show(information, "操作完成", "本次更改已经保存。", false);
        confirmation.Click += async (_, _) => await Show(confirmation, "确认归档此项目？", "项目将从活动列表移除，确认后本次动作执行一次。", false);
        form.Click += async (_, _) => await Show(form, "编辑个人资料", "更新姓名并确认保存。", true);
        async Task Show(Button trigger, string title, string description, bool hasForm)
        {
            if (TopLevel.GetTopLevel(this) is not Window owner) return;
            var dialog = new VhilzWindow { Title = title, Width = 460, Height = hasForm ? 390 : 270, CanResize = false,
                WindowStartupLocation = WindowStartupLocation.CenterOwner, RequestedThemeVariant = owner.ActualThemeVariant };
            var cancel = new Button { Content = "取消", IsCancel = true };
            var accept = new Button { Content = "确认", IsDefault = true };
            var body = Column(Text(title), Text(description));
            if (hasForm) body.Children.Add(new TextBox { Text = "Alex Chen", PlaceholderText = "姓名" });
            body.Children.Add(new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Spacing = 8, Children = { cancel, accept } });
            dialog.Content = new Border { Padding = new Thickness(24), Child = body };
            cancel.Click += (_, _) => dialog.Close(false);
            accept.Click += (_, _) => dialog.Close(true);
            dialog.KeyDown += (_, e) => { if (e.Key == Key.Escape) { dialog.Close(false); e.Handled = true; } };
            try { result.Text = await dialog.ShowDialog<bool>(owner) ? "已确认，动作执行一次" : "已取消或关闭"; }
            finally { trigger.Focus(); }
        }
        Content = Column(Text("Dialog / AlertDialog · 原生窗口组合"), information, confirmation, form, result);
    }

}
