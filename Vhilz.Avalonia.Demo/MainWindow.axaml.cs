using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Lucide.Avalonia;
using Vhilz.Avalonia.Theme.Controls;
using IconPath = Avalonia.Controls.Shapes.Path;

namespace Vhilz.Avalonia.Demo;

public partial class MainWindow : VhilzWindow
{
    private Control? _comparisonIcon;
    private int _buttonPreviewClickCount;

    public MainWindow()
    {
        InitializeComponent();
        Opened += (_, _) => UpdateIconComparison();
        Closing += (_, e) => {
            if (PreventClose.IsChecked == true) {
                e.Cancel = true;
                WindowBehaviorStatus.Text = "关闭请求已取消；取消勾选后可关闭窗口。";
            }
        };
    }

    private async void OnOpenDialog(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button trigger) return;
        trigger.IsEnabled = false;
        try {
            var accept = new Button { Content = "确认并返回" };
            var content = new StackPanel {
                Margin = new Thickness(20), Spacing = 12,
                Children = { new TextBlock { Text = "关闭此窗口后，主窗口应恢复交互。" }, accept }
            };
            content.Styles.Add(new global::Avalonia.Themes.Fluent.FluentTheme());
            var dialog = new VhilzWindow {
                Title = "模态窗口", Width = 400, Height = 180,
                ExtendClientAreaToDecorationsHint = true,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = content
            };
            accept.Click += (_, _) => dialog.Close(true);
            var accepted = await dialog.ShowDialog<bool>(this);
            WindowBehaviorStatus.Text = accepted ? "模态窗口已确认返回。" : "模态窗口已关闭。";
        }
        finally {
            trigger.IsEnabled = true;
        }
    }

    private void OnOpenComponentCatalog(object? sender, RoutedEventArgs e) {
        new Catalog.ComponentCatalogWindow().Show(this);
    }

    private void OnThemeSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox selector && Application.Current is { } application)
        {
            application.RequestedThemeVariant = selector.SelectedIndex switch
            {
                1 => ThemeVariant.Light,
                2 => ThemeVariant.Dark,
                _ => ThemeVariant.Default
            };
        }
    }

    private void OnIconComparisonChanged(object? sender, SelectionChangedEventArgs e) => UpdateIconComparison();

    // 图标信息按需读取，避免诊断文字和整树查找进入动画的每次布局。
    private void OnRefreshIconMetrics(object? sender, RoutedEventArgs e) => UpdateIconMetrics();

    private void OnTitleAlignmentChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox selector)
            TitleAlignment = selector.SelectedIndex == 1 ? TextAlignment.Center : TextAlignment.Left;
    }

    private void OnCaptionPreviewClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button)
            CaptionPreviewStatus.Text = $"{button.Tag}预览：点击已触发。";
    }

    private void OnButtonPreviewClick(object? sender, RoutedEventArgs e) {
        if (sender is Button button)
            ButtonPreviewStatus.Text = $"{button.Tag}：已触发 {++_buttonPreviewClickCount} 次点击。";
    }

    private void UpdateIconComparison()
    {
        var restore = FindRestoreIcon();
        if (restore?.Parent is not Grid slot || IconComparisonSelector is null)
            return;

        if (_comparisonIcon is not null)
            slot.Children.Remove(_comparisonIcon);
        _comparisonIcon = null;
        restore.Opacity = 1;

        var mode = IconComparisonSelector.SelectedIndex;
        if (mode <= 0)
            return;

        // 对照只在 Demo 中替换显示内容；按钮、位置、窗口状态和生产模板保持一致。
        if (mode == 1)
        {
            var path = new IconPath
            {
                Stretch = Stretch.None,
                StrokeLineCap = PenLineCap.Round,
                StrokeJoin = PenLineJoin.Round
            };
            path.Data = Geometry.Parse("M9 3h11a1 1 0 0 1 1 1v11 M4 8h11a1 1 0 0 1 1 1v11a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1v-11a1 1 0 0 1 1-1z");
            path.Bind(IconPath.StrokeProperty, new Binding(nameof(restore.Foreground)) { Source = restore });
            path.StrokeThickness = 2;
            var legacy = new Viewbox
            {
                Child = new Canvas { Width = 24, Height = 24, Children = { path } }
            };
            legacy.Bind(WidthProperty, new Binding(nameof(restore.Size)) { Source = restore });
            legacy.Bind(HeightProperty, new Binding(nameof(restore.Size)) { Source = restore });
            _comparisonIcon = legacy;
        }
        else
        {
            LucideIcon comparison = mode == 2
                ? new GeometryIcon { Data = Geometry.Parse(LucideIconKind.Square.GetGeometryData()) }
                : new LucideIcon { Kind = LucideIconKind.Square };
            comparison.Bind(LucideIcon.SizeProperty, new Binding(nameof(restore.Size)) { Source = restore });
            comparison.Bind(LucideIcon.StrokeWidthProperty, new Binding(nameof(restore.StrokeWidth)) { Source = restore });
            comparison.Bind(LucideIcon.ForegroundProperty, new Binding(nameof(restore.Foreground)) { Source = restore });
            _comparisonIcon = comparison;
        }

        _comparisonIcon.IsHitTestVisible = false;
        _comparisonIcon.Bind(IsVisibleProperty, new Binding(nameof(restore.IsVisible)) { Source = restore });
        restore.Opacity = 0;
        slot.Children.Add(_comparisonIcon);
        UpdateIconMetrics();
    }

    private GeometryIcon? FindRestoreIcon() =>
        // Avalonia 12 将窗口与装饰放在同一个 TopLevelHost 下，装饰不是窗口的视觉子项。
        (this.GetVisualAncestors().LastOrDefault() ?? this).GetVisualDescendants().OfType<GeometryIcon>()
            .FirstOrDefault(icon => icon.Name == "PartRestoreIcon");

    private void UpdateIconMetrics()
    {
        if (IconMetrics is null)
            return;
        var restore = FindRestoreIcon();
        if (restore is null)
        {
            IconMetrics.Text = "当前平台尚未生成自绘窗口装饰。";
            return;
        }
        var origin = restore.TranslatePoint(default, this);
        var visible = _comparisonIcon ?? restore;
        var comparisonOrigin = visible.TranslatePoint(default, this);
        IconMetrics.Text = $"窗口缩放 {RenderScaling:P0} · 图标 {restore.Size:0.##} DIP · " +
            $"线宽 {restore.Size * restore.StrokeWidth / 24:0.###} DIP\n" +
            $"原图标位置 {origin} · 当前对照位置 {comparisonOrigin} · 当前尺寸 {visible.Bounds.Size}";
    }
}
