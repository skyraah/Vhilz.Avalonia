using System;
using System.Collections.Generic;
using Avalonia.Controls;

namespace Vhilz.Avalonia.Demo.Catalog;

internal sealed record ComponentEntry(string Id, string Name, Func<Control> CreateSample) {
    public override string ToString() => $"{Id} · {Name}";
}

internal static class ComponentCatalog {
    public static IReadOnlyList<ComponentEntry> Entries { get; } = [
        new("N01", "Button 与 RepeatButton", () => new global::Vhilz.Avalonia.Demo.Components.N01.N01Sample()),
        new("N02", "文字与标签", () => new global::Vhilz.Avalonia.Demo.Components.N02.N02Sample()),
        new("N03", "Separator", () => new global::Vhilz.Avalonia.Demo.Components.N03.N03Sample()),
        new("N04", "ToggleButton", () => new global::Vhilz.Avalonia.Demo.Components.N04.N04Sample()),
        new("N05", "CheckBox", () => new global::Vhilz.Avalonia.Demo.Components.N05.N05Sample()),
        new("N06", "RadioButton 与分组示例", () => new global::Vhilz.Avalonia.Demo.Components.N06.N06Sample()),
        new("N07", "ToggleSwitch", () => new global::Vhilz.Avalonia.Demo.Components.N07.N07Sample()),
        new("N08", "ScrollViewer 与 ScrollBar", () => new global::Vhilz.Avalonia.Demo.Components.N08.N08Sample()),
        new("N09", "TextBox、MaskedTextBox 与校验提示", () => new global::Vhilz.Avalonia.Demo.Components.N09.N09Sample()),
        new("N10", "Slider", () => new global::Vhilz.Avalonia.Demo.Components.N10.N10Sample()),
        new("N11", "ProgressBar 与忙碌展示", () => new global::Vhilz.Avalonia.Demo.Components.N11.N11Sample()),
        new("N12", "Expander", () => new global::Vhilz.Avalonia.Demo.Components.N12.N12Sample()),
        new("N13", "GridSplitter 与分栏示例", () => new global::Vhilz.Avalonia.Demo.Components.N13.N13Sample()),
        new("N14", "ItemsControl 与 ListBox", () => new global::Vhilz.Avalonia.Demo.Components.N14.N14Sample()),
        new("N15", "ComboBox 与选择下拉", () => new global::Vhilz.Avalonia.Demo.Components.N15.N15Sample()),
        new("N16", "AutoCompleteBox", () => new global::Vhilz.Avalonia.Demo.Components.N16.N16Sample()),
        new("N17", "TabControl 与 TabStrip", () => new global::Vhilz.Avalonia.Demo.Components.N17.N17Sample()),
        new("N18", "ToolTip", () => new global::Vhilz.Avalonia.Demo.Components.N18.N18Sample()),
        new("N19", "Flyout 与 Popover", () => new global::Vhilz.Avalonia.Demo.Components.N19.N19Sample()),
        new("N20", "菜单家族", () => new global::Vhilz.Avalonia.Demo.Components.N20.N20Sample()),
        new("N21", "Calendar", () => new global::Vhilz.Avalonia.Demo.Components.N21.N21Sample()),
        new("N22", "CalendarDatePicker 与 DatePicker", () => new global::Vhilz.Avalonia.Demo.Components.N22.N22Sample()),
        new("N23", "Carousel", () => new global::Vhilz.Avalonia.Demo.Components.N23.N23Sample()),
        new("N24", "官方通知与 Toast", () => new global::Vhilz.Avalonia.Demo.Components.N24.N24Sample()),
        new("N25", "SplitView", () => new global::Vhilz.Avalonia.Demo.Components.N25.N25Sample()),
        new("N26", "TableView 与表格场景", () => new global::Vhilz.Avalonia.Demo.Components.N26.N26Sample()),
        new("N27", "Dialog 与 AlertDialog 的原生窗口方案", () => new global::Vhilz.Avalonia.Demo.Components.N27.N27Sample()),
        new("N28", "Direction 与原生流向", () => new global::Vhilz.Avalonia.Demo.Components.N28.N28Sample()),
        new("E01", "TreeView", () => new global::Vhilz.Avalonia.Demo.Components.E01.E01Sample()),
        new("E02", "NumericUpDown", () => new global::Vhilz.Avalonia.Demo.Components.E02.E02Sample()),
        new("E03", "TimePicker", () => new global::Vhilz.Avalonia.Demo.Components.E03.E03Sample()),
        new("E05", "其他原生按钮", () => new global::Vhilz.Avalonia.Demo.Components.E05.E05Sample()),
        new("E06", "GroupBox 与带标题容器", () => new global::Vhilz.Avalonia.Demo.Components.E06.E06Sample()),
        new("E07", "CommandBar", () => new global::Vhilz.Avalonia.Demo.Components.E07.E07Sample()),
        new("E08", "ContentPage", () => new global::Vhilz.Avalonia.Demo.Components.E08.E08Sample()),
        new("E09", "NavigationPage", () => new global::Vhilz.Avalonia.Demo.Components.E09.E09Sample()),
        new("E10", "TabbedPage", () => new global::Vhilz.Avalonia.Demo.Components.E10.E10Sample()),
        new("E11", "DrawerPage", () => new global::Vhilz.Avalonia.Demo.Components.E11.E11Sample()),
        new("E12", "CarouselPage", () => new global::Vhilz.Avalonia.Demo.Components.E12.E12Sample()),
        new("E13", "PipsPager", () => new global::Vhilz.Avalonia.Demo.Components.E13.E13Sample()),
        new("E14", "RefreshContainer", () => new global::Vhilz.Avalonia.Demo.Components.E14.E14Sample())
    ];
}
