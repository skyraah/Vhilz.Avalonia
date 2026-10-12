using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Vhilz.Avalonia.Theme.Controls;
using static Vhilz.Avalonia.Demo.Components.NativeSampleLayout;

namespace Vhilz.Avalonia.Demo.Components.Windows;

public sealed class WindowSample : UserControl {
    private VhilzWindow? _preview;

    public WindowSample() {
        Content = Column(
            Text("VhilzWindow 提供窗口，TitleBar 承载标题与自定义内容，窗口装饰提供最小化、最大化 / 还原、全屏和关闭按钮。"),
            Text("打开示例窗口后，可以一起调整标题栏与按钮，并实际拖动、缩放和切换窗口状态。明暗主题跟随目录工具栏。"),
            Action("打开示例窗口", OpenPreview));
    }

    private void OpenPreview() {
        if (_preview is not null) {
            if (_preview.WindowState == WindowState.Minimized) _preview.WindowState = WindowState.Normal;
            _preview.Activate();
            return;
        }

        if (TopLevel.GetTopLevel(this) is not Window owner) return;

        var window = new VhilzWindow {
            Title = "我的应用", Width = 760, Height = 640, MinWidth = 540, MinHeight = 440,
            ExtendClientAreaToDecorationsHint = true,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            IsFullScreenButtonVisible = true,
            LeftContent = new TextBlock { Text = "工作区", VerticalAlignment = VerticalAlignment.Center },
            CenterContent = new TextBlock { Text = "已保存", VerticalAlignment = VerticalAlignment.Center }
        };
        _preview = window;
        window.Bind(TopLevel.RequestedThemeVariantProperty,
            new Binding(nameof(TopLevel.RequestedThemeVariant)) { Source = owner });
        var status = Text("点击标题栏中的“帮助”，或操作窗口按钮。全屏时按 Esc 返回。");
        window.RightContent = Action("帮助", () => status.Text = "标题栏内容可以交互；其余标题区域可用于拖动窗口。");
        var title = new TextBox { Text = window.Title, PlaceholderText = "窗口标题" };
        window.Bind(Window.TitleProperty, new Binding(nameof(TextBox.Text)) { Source = title });
        var body = Column(
            Text("窗口标题"), title,
            Text("标题栏内容 · 左侧工作区 / 中间保存状态 / 右侧帮助"),
            Option("显示标题文字", true, value => window.IsTitleVisible = value),
            Option("显示自定义标题栏", true, value => window.IsTitleBarVisible = value),
            Text("窗口按钮 · 在真实标题栏中检查悬停、按下和焦点状态"),
            Option("显示最小化按钮", true, value => window.IsMinimizeButtonVisible = value),
            Option("显示最大化 / 还原按钮", true, value => window.IsRestoreButtonVisible = value),
            Option("显示全屏按钮", true, value => window.IsFullScreenButtonVisible = value),
            Option("显示关闭按钮", true, value => window.IsCloseButtonVisible = value),
            Option("允许最小化", true, value => window.CanMinimize = value),
            Option("允许调整窗口尺寸", true, value => window.CanResize = value),
            status, Action("关闭示例窗口", window.Close));
        window.Content = new ScrollViewer { Content = new Border { Padding = new Thickness(24), Child = body } };
        window.Closed += (_, _) => _preview = null;
        window.Show(owner);
    }

    private static CheckBox Option(string label, bool initial, Action<bool> apply) {
        var option = new CheckBox { Content = label, IsChecked = initial };
        option.IsCheckedChanged += (_, _) => apply(option.IsChecked == true);
        return option;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) {
        _preview?.Close();
        base.OnDetachedFromVisualTree(e);
    }
}
