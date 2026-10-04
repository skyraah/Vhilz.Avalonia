using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Vhilz.Avalonia.Theme.Controls;

// 选择当前目标状态的时长或缓动；保留 Transition 实例，让反向过渡接续当前颜色。
internal sealed class CaptionTransitionConverter : IMultiValueConverter {
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture) {
        // :pressed 样式先于 IsPressed 通知更新，因此以目标画笔选择时长。
        if (values.Count == 7 && values[3] is Classes classes) {
            if (ReferenceEquals(values[0], values[1]) || ReferenceEquals(values[0], values[2]))
                return values[5] ?? AvaloniaProperty.UnsetValue;

            var closeExit = classes.Contains("VhilzCaptionClose") &&
                            values[0] is ISolidColorBrush { Color.A: 0 };
            return values[closeExit ? 6 : 4] ?? AvaloniaProperty.UnsetValue;
        }

        return AvaloniaProperty.UnsetValue;
    }
}
