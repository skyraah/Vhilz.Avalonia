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

    public MainWindow()
    {
        InitializeComponent();
        Opened += (_, _) => UpdateIconComparison();
        LayoutUpdated += (_, _) => UpdateIconMetrics();
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

    private void OnCaptionPreviewClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button)
            CaptionPreviewStatus.Text = $"{button.Tag}预览：点击已触发。";
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
                ? new CaptionGeometryIcon { Data = Geometry.Parse(LucideIconKind.Square.GetGeometryData()) }
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

    private CaptionGeometryIcon? FindRestoreIcon() =>
        // Avalonia 12 将窗口与装饰放在同一个 TopLevelHost 下，装饰不是窗口的视觉子项。
        (this.GetVisualAncestors().LastOrDefault() ?? this).GetVisualDescendants().OfType<CaptionGeometryIcon>()
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
