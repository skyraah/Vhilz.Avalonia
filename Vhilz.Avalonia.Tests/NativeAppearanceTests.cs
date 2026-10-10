using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Vhilz.Avalonia.Theme;
using Xunit;

namespace Vhilz.Avalonia.Tests;

public class NativeAppearanceTests {
    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void TextEditingValidationAndResourceOverridesSurviveThemeChanges(bool light) {
        var input = new TextBox { Text = "测试 input", Width = 320 };
        var window = new Window {
            Width = 480, Height = 240, Content = input,
            RequestedThemeVariant = light ? ThemeVariant.Light : ThemeVariant.Dark
        };
        window.Show();
        try {
            Assert.Contains(input.GetVisualDescendants(),
                control => control is TextPresenter { Name: "PART_TextPresenter" });
            Assert.Contains(input.GetVisualDescendants(),
                control => control is ScrollViewer { Name: "PART_ScrollViewer" });
            input.Focus();
            input.SelectAll();
            window.KeyTextInput("中文输入");
            Assert.Equal("中文输入", input.Text);
            input.Undo();
            Assert.Equal("测试 input", input.Text);
            input.IsReadOnly = true;
            window.KeyTextInput("不会写入");
            Assert.Equal("测试 input", input.Text);

            DataValidationErrors.SetErrors(input, new[] { "示例校验错误" });
            Assert.True(DataValidationErrors.GetHasErrors(input));
            Assert.Equal(FindColor(input, ResourceKeys.Brush.Danger), SolidColor(input.BorderBrush));
            var ring = Assert.Single(input.GetVisualDescendants().OfType<Border>(),
                border => border.Name == "PART_FocusRing");
            Assert.True(ring.IsVisible);
            Assert.False(ring.IsHitTestVisible);
            window.RequestedThemeVariant = light ? ThemeVariant.Dark : ThemeVariant.Light;
            Assert.Equal(FindColor(input, ResourceKeys.Brush.Danger), SolidColor(input.BorderBrush));
            DataValidationErrors.SetErrors(input, null);
            Assert.False(DataValidationErrors.GetHasErrors(input));

            Application.Current!.Resources[ResourceKeys.Color.Input] = Colors.Navy;
            Assert.Equal(Colors.Navy, SolidColor(input.Background));
            input.Background = Brushes.Maroon;
            Application.Current.Resources[ResourceKeys.Color.Input] = Colors.Teal;
            Assert.Same(Brushes.Maroon, input.Background);
        }
        finally {
            Application.Current!.Resources.Remove(ResourceKeys.Color.Input);
            window.Close();
        }
    }

    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void ScrollViewerMeasuresScrollableContentAndKeepsItsNativeParts(bool light) {
        var viewer = new ScrollViewer {
            Content = new Border { Width = 900, Height = 900 },
            HorizontalScrollBarVisibility = global::Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = global::Avalonia.Controls.Primitives.ScrollBarVisibility.Auto
        };
        var window = new Window {
            Width = 320, Height = 240, Content = viewer,
            RequestedThemeVariant = light ? ThemeVariant.Light : ThemeVariant.Dark
        };
        window.Show();
        try {
            Assert.True(viewer.Extent.Width > viewer.Viewport.Width);
            Assert.True(viewer.Extent.Height > viewer.Viewport.Height);
            Assert.Contains(viewer.GetVisualDescendants(), control => control is ScrollContentPresenter);
            viewer.Offset = new Vector(120, 140);
            window.UpdateLayout();
            Assert.Equal(new Vector(120, 140), viewer.Offset);
            Assert.Equal(2,
                viewer.GetVisualDescendants().OfType<global::Avalonia.Controls.Primitives.ScrollBar>().Count());
            var horizontal = viewer.GetVisualDescendants().OfType<global::Avalonia.Controls.Primitives.ScrollBar>()
                .Single(bar => bar.Orientation == Orientation.Horizontal);
            var pageButtons = horizontal.GetVisualDescendants().OfType<RepeatButton>()
                .Where(button => button.Name is "PART_PageUpButton" or "PART_PageDownButton").ToArray();
            Assert.Equal(2, pageButtons.Length);
            Assert.All(pageButtons, button => {
                Assert.Equal(0, SolidColor(button.Background).A);
                Assert.True(button.Bounds.Height <= horizontal.Bounds.Height);
                Assert.Equal(HorizontalAlignment.Stretch, button.HorizontalAlignment);
            });
        }
        finally {
            window.Close();
        }
    }

    private static Color FindColor(Control control, string key) =>
        SolidColor(Assert.IsAssignableFrom<IBrush>(control.FindResource(control.ActualThemeVariant, key)));

    private static Color SolidColor(IBrush? brush) => Assert.IsAssignableFrom<ISolidColorBrush>(brush).Color;

    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void CheckboxKeepsThreeStatesAndDisabledKeyboardBehavior(bool light) {
        var checkbox = new CheckBox { Content = "允许同步", IsThreeState = true, IsChecked = false };
        var window = new Window {
            Width = 360, Height = 160, Content = checkbox,
            RequestedThemeVariant = light ? ThemeVariant.Light : ThemeVariant.Dark
        };
        window.Show();
        try {
            checkbox.Focus(NavigationMethod.Tab);
            var focus = Assert.Single(checkbox.GetVisualDescendants().OfType<Border>(),
                border => border.Name == "PART_FocusRing");
            Assert.True(focus.IsVisible);
            foreach (var expected in new bool?[] { true, null, false }) {
                window.KeyPress(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null);
                window.KeyRelease(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null);
                Assert.Equal(expected, checkbox.IsChecked);
                var marks = checkbox.GetVisualDescendants().OfType<Control>()
                    .Where(control => control.Name is "checkMark" or "indeterminateMark").ToArray();
                Assert.Equal(2, marks.Length);
                Assert.Equal(expected == true, marks.Single(mark => mark.Name == "checkMark").IsVisible);
                Assert.Equal(expected is null, marks.Single(mark => mark.Name == "indeterminateMark").IsVisible);
            }

            checkbox.IsEnabled = false;
            window.KeyPress(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null);
            window.KeyRelease(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null);
            Assert.False(checkbox.IsChecked);
            Assert.False(focus.IsVisible);
        }
        finally {
            window.Close();
        }
    }

    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void HoverKeepsToggleSelectionVisibleAndCompletedProgressTextReadable(bool light) {
        var selected = new global::Avalonia.Controls.Primitives.ToggleButton { Content = "已选中", IsChecked = true };
        var ordinary = new global::Avalonia.Controls.Primitives.ToggleButton { Content = "未选中" };
        var progress = new ProgressBar { Value = 100, ShowProgressText = true, Height = 24 };
        var window = new Window {
            Width = 420, Height = 240,
            RequestedThemeVariant = light ? ThemeVariant.Light : ThemeVariant.Dark,
            Content = new StackPanel { Spacing = 12, Children = { selected, ordinary, progress } }
        };
        window.Show();
        try {
            window.MouseMove(selected.TranslatePoint(new Point(5, 5), window)!.Value);
            Assert.True(selected.IsPointerOver);
            var selectedHover = (SolidColor(selected.Background), SolidColor(selected.BorderBrush));
            window.MouseMove(ordinary.TranslatePoint(new Point(5, 5), window)!.Value);
            Assert.True(ordinary.IsPointerOver);
            Assert.NotEqual(selectedHover, (SolidColor(ordinary.Background), SolidColor(ordinary.BorderBrush)));
            Assert.True(selected.IsChecked);
            var textHost = progress.GetVisualDescendants().OfType<LayoutTransformControl>()
                .Single(control => control.Name == "PART_LayoutTransformControl");
            var surface = Assert.IsType<Border>(textHost.Child);
            var text = Assert.IsType<TextBlock>(surface.Child);
            Assert.NotEqual(SolidColor(surface.Background), SolidColor(text.Foreground));
            Assert.Contains("100", text.Text);
        }
        finally {
            window.Close();
        }
    }
}
