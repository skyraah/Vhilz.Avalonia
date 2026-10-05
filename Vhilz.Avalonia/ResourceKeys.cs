namespace Vhilz.Avalonia.Theme;

/// <summary>资源键集中入口；语义 Token 优先，控件键只用于独立覆盖的差异。</summary>
public static class ResourceKeys {
    public static class Brush {
        public const string Border = "Vhilz.Brush.Border";
        public const string BorderInactive = "Vhilz.Brush.BorderInactive";
        public const string OnDanger = "Vhilz.Brush.OnDanger";
        public const string Surface = "Vhilz.Brush.Surface";
        public const string Text = "Vhilz.Brush.Text";
        public const string TextMuted = "Vhilz.Brush.TextMuted";
        public const string TextSecondary = "Vhilz.Brush.TextSecondary";
    }
    public static class CaptionButton {
        public static class Close {
            public static class Duration {
                public const string Reset = "Vhilz.CaptionButton.Close.Duration.Reset";
            }
            public static class PointerOver {
                public const string BackgroundBrush = "Vhilz.CaptionButton.Close.PointerOver.BackgroundBrush";
            }
            public static class Pressed {
                public const string BackgroundBrush = "Vhilz.CaptionButton.Close.Pressed.BackgroundBrush";
            }
        }
        public static class Duration {
            public const string Color = "Vhilz.CaptionButton.Duration.Color";
        }
        public static class Group {
            public const string Margin = "Vhilz.CaptionButton.Group.Margin";
        }
        public static class Icon {
            public static class Maximize {
                public const string Size = "Vhilz.CaptionButton.Icon.Maximize.Size";
            }
            public static class Restore {
                public const string Geometry = "Vhilz.CaptionButton.Icon.Restore.Geometry";
                public const string StrokeWidth = "Vhilz.CaptionButton.Icon.Restore.StrokeWidth";
            }
            public const string Size = "Vhilz.CaptionButton.Icon.Size";
            public const string StrokeWidth = "Vhilz.CaptionButton.Icon.StrokeWidth";
            public const string Theme = "Vhilz.CaptionButton.Icon.Theme";
        }
        public const string Margin = "Vhilz.CaptionButton.Margin";
        public static class PointerOver {
            public const string BackgroundBrush = "Vhilz.CaptionButton.PointerOver.BackgroundBrush";
        }
        public static class Pressed {
            public const string BackgroundBrush = "Vhilz.CaptionButton.Pressed.BackgroundBrush";
        }
        public static class RevealBorder {
            public const string Color = "Vhilz.CaptionButton.RevealBorder.Color";
            public const string Intensity = "Vhilz.CaptionButton.RevealBorder.Intensity";
            public const string IsEnabled = "Vhilz.CaptionButton.RevealBorder.IsEnabled";
            public const string ProximityDistance = "Vhilz.CaptionButton.RevealBorder.ProximityDistance";
            public const string Radius = "Vhilz.CaptionButton.RevealBorder.Radius";
            public const string Width = "Vhilz.CaptionButton.RevealBorder.Width";
        }
        public static class Surface {
            public static class Pressed {
                public const string RenderTransform = "Vhilz.CaptionButton.Surface.Pressed.RenderTransform";
            }
        }
        public const string Theme = "Vhilz.CaptionButton.Theme";
        public const string Width = "Vhilz.CaptionButton.Width";
    }
    public static class Color {
        public const string Border = "Vhilz.Color.Border";
        public const string BorderInactive = "Vhilz.Color.BorderInactive";
        public const string Surface = "Vhilz.Color.Surface";
        public const string Text = "Vhilz.Color.Text";
        public const string TextMuted = "Vhilz.Color.TextMuted";
        public const string TextSecondary = "Vhilz.Color.TextSecondary";
    }
    public static class Duration {
        public const string Fast = "Vhilz.Duration.Fast";
        public const string Normal = "Vhilz.Duration.Normal";
        public const string Slow = "Vhilz.Duration.Slow";
    }
    public static class Easing {
        public const string Decelerate = "Vhilz.Easing.Decelerate";
        public const string Spring = "Vhilz.Easing.Spring";
        public const string Standard = "Vhilz.Easing.Standard";
    }
    public static class Opacity {
        public const string Disabled = "Vhilz.Opacity.Disabled";
    }
    internal static class Preview {
        public static class WindowDecorations {
            public const string BackgroundBrush = "Vhilz.Preview.WindowDecorations.BackgroundBrush";
            public const string ForegroundBrush = "Vhilz.Preview.WindowDecorations.ForegroundBrush";
            public static class Row {
                public const string DataTemplate = "Vhilz.Preview.WindowDecorations.Row.DataTemplate";
            }
        }
    }
    public static class Radius {
        public const string Small = "Vhilz.Radius.Small";
    }
    public static class Text {
        public static class Window {
            public const string Close = "Vhilz.Text.Window.Close";
            public const string ExitFullscreen = "Vhilz.Text.Window.ExitFullscreen";
            public const string Fullscreen = "Vhilz.Text.Window.Fullscreen";
            public const string Maximize = "Vhilz.Text.Window.Maximize";
            public const string Minimize = "Vhilz.Text.Window.Minimize";
            public const string Restore = "Vhilz.Text.Window.Restore";
        }
    }
    public static class Thickness {
        public const string Focus = "Vhilz.Thickness.Focus";
    }
    public static class TitleBar {
        public const string Theme = "Vhilz.TitleBar.Theme";
    }
    public static class Window {
        public const string BackgroundBrush = "Vhilz.Window.BackgroundBrush";
        public const string BorderBrush = "Vhilz.Window.BorderBrush";
        public const string ForegroundBrush = "Vhilz.Window.ForegroundBrush";
        public const string FrameThickness = "Vhilz.Window.FrameThickness";
        public static class FullscreenTitleBar {
            public const string Margin = "Vhilz.Window.FullscreenTitleBar.Margin";
        }
        public static class Inactive {
            public const string BorderBrush = "Vhilz.Window.Inactive.BorderBrush";
        }
        public const string Theme = "Vhilz.Window.Theme";
        public static class TitleBar {
            public const string BackgroundBrush = "Vhilz.Window.TitleBar.BackgroundBrush";
            public const string ForegroundBrush = "Vhilz.Window.TitleBar.ForegroundBrush";
            public const string Height = "Vhilz.Window.TitleBar.Height";
            public static class Inactive {
                public const string ForegroundBrush = "Vhilz.Window.TitleBar.Inactive.ForegroundBrush";
            }
            public static class Title {
                public const string Margin = "Vhilz.Window.TitleBar.Title.Margin";
                public const string Theme = "Vhilz.Window.TitleBar.Title.Theme";
            }
        }
        public static class TransparencyFallback {
            public const string BackgroundBrush = "Vhilz.Window.TransparencyFallback.BackgroundBrush";
        }
    }
    public static class WindowDecorations {
        public const string Theme = "Vhilz.WindowDecorations.Theme";
    }
}
