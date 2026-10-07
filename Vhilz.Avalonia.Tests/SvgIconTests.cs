using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using SkiaSharp;
using Vhilz.Avalonia.Theme.Controls;
using Xunit;

namespace Vhilz.Avalonia.Tests;

public class SvgIconTests
{
    private const string MulticolorSvg = """
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 32 16">
          <rect width="16" height="16" fill="currentColor" />
          <rect x="16" width="16" height="16" fill="#F59E0B" />
        </svg>
        """;

    [AvaloniaFact]
    public void FileAndFileUriPreserveColorsAndFollowInheritedMutableForeground()
    {
        var file = Path.GetTempFileName();
        File.WriteAllText(file, MulticolorSvg);
        var brush = new SolidColorBrush(Colors.White);
        var icon = new SvgIcon();
        var host = new Button { Foreground = brush, Content = icon };
        try
        {
            foreach (var path in new[] { file, new Uri(file).AbsoluteUri })
            {
                icon.Path = path;
                AssertPixels(icon, SKColors.White);
                brush.Color = Colors.Black;
                AssertPixels(icon, SKColors.Black);
                host.Foreground = Brushes.Red;
                AssertPixels(icon, SKColors.Red);
                brush.Color = Colors.White;
                host.Foreground = brush;
            }

            icon.Path = null;
            Assert.Null(icon.Picture);
        }
        finally
        {
            icon.Path = null;
            File.Delete(file);
        }
    }

    [AvaloniaFact]
    public void SizeReservesSquareSlotAndPathReplacementDoesNotRetainPreviousDrawing()
    {
        var file = Path.GetTempFileName();
        File.WriteAllText(file, MulticolorSvg);
        var icon = new SvgIcon { Path = file };
        try
        {
            Assert.Equal(Stretch.Uniform, icon.Stretch);
            foreach (var size in new[] { 24d, 16d, 32d })
            {
                icon.Size = size;
                icon.Measure(new Size(100, 100));
                icon.Arrange(new Rect(0, 0, size, size));
                Assert.Equal(new Size(size, size), icon.DesiredSize);
                Assert.Equal(new Size(size, size), icon.Bounds.Size);
            }

            icon.Path = file + ".missing.svg";
            Assert.Null(icon.Picture);
            icon.Path = file;
            Assert.NotNull(icon.Picture);
        }
        finally
        {
            icon.Path = null;
            File.Delete(file);
        }
    }

    private static void AssertPixels(SvgIcon icon, SKColor foreground)
    {
        Assert.NotNull(icon.Picture);
        using var bitmap = new SKBitmap(32, 16);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.Transparent);
        canvas.DrawPicture(icon.Picture);
        Assert.Equal(foreground, bitmap.GetPixel(8, 8));
        Assert.Equal(new SKColor(0xF5, 0x9E, 0x0B), bitmap.GetPixel(24, 8));
    }
}
