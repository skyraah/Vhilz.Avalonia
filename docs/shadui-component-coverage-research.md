# ShadUI 与 Avalonia 官方控件覆盖对照

> 调研快照：2026-10-07。本文记录指定版本的源码事实，供控件规划参考，不代表 Vhilz 已实现这些能力，也不是视觉或交互验收报告。

## 范围与结论

这里的“官方”指 **Avalonia 官方控件**，不是 React 的 shadcn/ui。

- ShadUI 基准：`main` 在调研时指向 [
  `ff1e84698d1478fc6403175858a2fd5303ad28df`](https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df)
  。其 [ShadUI.csproj][project] 依赖 Avalonia、SimpleTheme、ColorPicker、DataGrid **12.1.0**。
- 官方基准：Avalonia **12.1.0** 的控件源码和官方主题；本仓库使用 **12.1.3**，不能沿用 Avalonia 11 的旧目录来判断覆盖。
- **ShadUI 没有完成官方控件的专属主题全覆盖。** 它在 [Styles.axaml][styles] 中加载 `SimpleTheme`
  ，没有专属实现的许多控件由官方主题兜底。兜底不等于不能用，也不等于外观已经统一。
- 静态枚举得到 **42 个官方类型的全局 `ControlTheme`**，另有 `ScrollViewer`、`ScrollBar`、`Separator` 三个官方类型的选择器样式，共
  **45 个官方类型有直接主题/样式定义**。这里包含部件，不是 45 个独立产品组件，也不是功能完整度百分比。
- ShadUI 库中有 **21 个自定义控件类**：19 个公开类、2 个内部类。它们都是基于 Avalonia 继承或组合实现；“自研”只表示库定义了自己的控件类型和
  API，不表示脱离 Avalonia 从零实现。

## 分类口径

| 标记     | 含义                                                                                         |
|----------|----------------------------------------------------------------------------------------------|
| 原生换肤 | 使用 Avalonia 原控件类型，ShadUI 定义模板、资源或样式；可附加辅助行为。                      |
| 原生扩展 | 定义新类型，继承具有明确用途的原生控件，如 `Window`、`RadioButton`。归入自定义控件。         |
| 自研组合 | 基于 `TemplatedControl`、`ContentControl`、`ItemsControl` 等基础类组合出新控件。             |
| 间接覆盖 | 通过官方的样式键继承或已换肤的子部件受益，未定义独立全局主题。                               |
| 官方回退 | 没有 ShadUI 专属全局主题，使用其加载的官方 `SimpleTheme`；局部子部件仍可能采用 ShadUI 样式。 |
| 未集成   | 当前 ShadUI 库没有对应依赖与适配。                                                           |

“已有主题”只证明源码中存在并加载了相应定义，不证明每个属性、状态、键盘行为和平台表现都已验证。Demo
页面名称、目录名称和模板中出现过某控件，都不能单独作为覆盖证据。以下路径链接固定到上述提交；主题加载以 [Resources.axaml][resources]、[Styles.axaml][styles]
为准。

## 原生控件：ShadUI 已直接提供主题或样式

| ShadUI 名称/功能       | Avalonia 原生类型                                                                                    | 分类               | 实现与边界                                                                                                         |
|------------------------|------------------------------------------------------------------------------------------------------|--------------------|--------------------------------------------------------------------------------------------------------------------|
| Button                 | `Button`                                                                                             | 原生换肤           | [Button][button]；不同 Classes 是样式变体，不是不同控件。`ButtonAssist` 提供图标、忙碌等附加能力。                 |
| Toggle                 | `ToggleButton`                                                                                       | 原生换肤           | [ToggleButton][toggle]；没有独立的 `ShadUI.Toggle` 控件类。                                                        |
| Switch                 | `ToggleSwitch`                                                                                       | 原生换肤           | [Switch][switch]；目录叫 Switch，实际目标类型是 `ToggleSwitch`。                                                   |
| CheckBox               | `CheckBox`                                                                                           | 原生换肤           | [CheckBox][checkbox]。                                                                                             |
| RadioButton            | `RadioButton`                                                                                        | 原生换肤           | [RadioButton][radio]。                                                                                             |
| Input / TextBox        | `TextBox`                                                                                            | 原生换肤           | [TextBox][textbox]；标签、清除、格式化等附加能力并不使它变成独立 Input 类型。                                      |
| AutoCompleteBox        | `AutoCompleteBox`                                                                                    | 原生换肤           | [AutoCompleteBox][autocomplete]。                                                                                  |
| Numeric                | `NumericUpDown`、`ButtonSpinner`                                                                     | 原生换肤           | [NumericUpDown][numeric]；同时重写旋钮部件，提供 `NumericUpDownAssist`。                                           |
| ComboBox               | `ComboBox`、`ComboBoxItem`                                                                           | 原生换肤           | [ComboBox][combo]。                                                                                                |
| Calendar               | `Calendar`、`CalendarItem`、`CalendarButton`、`CalendarDayButton`                                    | 原生换肤           | [Calendar][calendar]；后三项是日历配套部件。                                                                       |
| 日历下拉日期选择       | `CalendarDatePicker`                                                                                 | 原生换肤           | [CalendarDatePicker][calendardate]；**不是**官方独立的 `DatePicker`。                                              |
| TimePicker             | `TimePicker`、`TimePickerPresenter`                                                                  | 原生换肤           | [TimePicker][timepicker]；包含时间弹层展示器。                                                                     |
| Slider                 | `Slider`                                                                                             | 原生换肤           | [Slider][slider]；其中 Thumb/RepeatButton 使用局部主题，不能据此称为全局覆盖。                                     |
| ListBox                | `ListBox`、`ListBoxItem`                                                                             | 原生换肤           | [ListBox][listbox]。                                                                                               |
| Tabs                   | `TabControl`、`TabItem`                                                                              | 原生换肤           | [TabControl][tabs]；另有 `TabControlBehaviors` 实现选中指示器动画；不等于覆盖独立 `TabStrip`。                     |
| Menu                   | `Menu`、`MenuItem`                                                                                   | 原生换肤           | [Menu][menu]；未定义 `ContextMenu` 的全局主题。                                                                    |
| MenuFlyout             | `MenuFlyoutPresenter`                                                                                | 原生换肤           | [MenuFlyoutPresenter][menuflyout]；通过展示器和菜单项覆盖菜单弹层视觉；`MenuFlyout` 本身不是一个重写的 ShadUI 类。 |
| ToolTip                | `ToolTip`                                                                                            | 原生换肤           | [ToolTip][tooltip]。                                                                                               |
| Typography / TextBlock | `TextBlock`                                                                                          | 原生换肤           | [TextBlock][textblock]；标题、正文等通过 Classes 配置。                                                            |
| SelectableTextBlock    | `SelectableTextBlock`                                                                                | 原生换肤           | [SelectableTextBlock][selectable]。                                                                                |
| Label                  | `Label`                                                                                              | 原生换肤           | [Label][label]。                                                                                                   |
| 输入校验提示           | `DataValidationErrors`                                                                               | 原生换肤           | [DataValidation][validation]；不是另造校验体系。                                                                   |
| Smooth Scroll          | `ScrollViewer`、`ScrollBar`                                                                          | 原生换肤＋附加行为 | [ScrollViewer][scroll]；选择器重写模板，并启用 `SmoothScrollAssist`，不是独立 SmoothScroll 控件。                  |
| Separator              | `Separator`                                                                                          | 原生样式           | [Separator][separator]；仅设置背景和外边距，模板仍使用基础主题。                                                   |
| Data Table             | `DataGrid`                                                                                           | 官方独立包换肤     | [DataGrid][datagrid]；使用 `Avalonia.Controls.DataGrid`，没有自研 DataTable 控件。                                 |
| DataGrid 部件          | `DataGridCell`、`DataGridColumnHeader`、`DataGridRowHeader`、`DataGridRow`、`DataGridRowGroupHeader` | 官方独立包换肤     | 同上，均有类型主题。不要与新核心控件 `TableView` 或树形表格 `TreeDataGrid` 混淆。                                  |
| ColorPicker            | `ColorPicker`、`ColorView`、`ColorSpectrum`、`ColorSlider`、`ColorPreviewer`                         | 官方独立包换肤     | [ColorPicker][color]；使用 `Avalonia.Controls.ColorPicker`，不是 ShadUI 自研颜色选择器。                           |

以上逐类型合计 45 项；把日历、列表、表格等部件合并后是 27 行功能组，不能将两种数量混用。

## 自定义控件：原生扩展与自研组合

| ShadUI 类型            | 直接基类                   | 分类     | 与官方控件的关系                                                                                                                                                    |
|------------------------|----------------------------|----------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `Window`               | `Avalonia.Controls.Window` | 原生扩展 | [Window][window]；自定义窗口属性、标题栏和模板。实际命名空间是 `ShadUI`，必须使用这个派生类型才能取得对应主题，不能认为普通 `Avalonia.Controls.Window` 被整体重写。 |
| `SidebarItem`          | `RadioButton`              | 原生扩展 | [Sidebar][sidebar]；复用单选行为，扩展图标和展开等 API；不是 `TreeViewItem`。                                                                                       |
| `BreakpointView`       | `Grid`                     | 原生扩展 | [Breakpoint][breakpoint]；根据视口宽度和断点决定显隐。                                                                                                              |
| `BreakpointViewPort`   | `Panel`                    | 原生扩展 | 同上；为断点提供祖先容器，没有单独 ControlTheme。                                                                                                                   |
| `Avatar`               | `TemplatedControl`         | 自研组合 | [Avatar][avatar]；头像组件，基类不是一个官方 Avatar。                                                                                                               |
| `Badge`                | `ContentControl`           | 自研组合 | [Badge][badge]；徽标内容组件。                                                                                                                                      |
| `Card`                 | `ContentControl`           | 自研组合 | [Card][card]；卡片容器。                                                                                                                                            |
| `CardTitle`            | `ContentControl`           | 自研组合 | 同上；卡片标题。                                                                                                                                                    |
| `CardDescription`      | `ContentControl`           | 自研组合 | 同上；卡片描述。                                                                                                                                                    |
| `BusyArea`             | `ContentControl`           | 自研组合 | [BusyArea][busy]；忙碌遮罩，与官方进度控件不是同一类型。                                                                                                            |
| `Loading`              | `TemplatedControl`         | 自研组合 | [Loading][loading]；加载动画，不能视为 `ProgressBar` 换肤或其确定进度 API 的覆盖。                                                                                  |
| `Skeleton`             | `TemplatedControl`         | 自研组合 | [Skeleton][skeleton]；骨架占位组件。                                                                                                                                |
| `DateInput`            | `TemplatedControl`         | 自研组合 | [DateInput][dateinput]；日期分段输入，既不继承 `DatePicker`，也不继承 `CalendarDatePicker`。                                                                        |
| `TimeInput`            | `TemplatedControl`         | 自研组合 | [TimeInput][timeinput]；时间分段输入，不是官方 `TimePicker` 的派生类。                                                                                              |
| `SimpleDropdown`       | `ItemsControl`             | 自研组合 | [SimpleDropdown][dropdown]；自己定义触发内容和展开 API，不是 `ComboBox`、`ContextMenu` 或 `SplitButton` 的换肤。                                                    |
| `Sidebar`              | `ContentControl`           | 自研组合 | [Sidebar][sidebar]；侧栏容器，不等于 `SplitView`、`TreeView` 或官方页面导航控件。                                                                                   |
| `SidebarItemLabel`     | `TemplatedControl`         | 自研组合 | 同上；侧栏标签。                                                                                                                                                    |
| `DialogHost`           | `TemplatedControl`         | 自研组合 | [Dialog][dialog]；配合 `DialogManager` 与 Builder 管理应用内对话框；不是系统原生文件对话框。                                                                        |
| `SimpleDialog`（内部） | `TemplatedControl`         | 自研组合 | 同上；内部默认对话框，不是公开可独立使用的控件 API。                                                                                                                |
| `ToastHost`            | `ItemsControl`             | 自研组合 | [Toast][toast]；配合 `ToastManager` 与 Builder 管理通知。                                                                                                           |
| `Toast`（内部）        | `ContentControl`           | 自研组合 | 同上；未继承官方 `NotificationCard` 或 `WindowNotificationManager`；功能相近不等于覆盖官方通知类型。                                                                |

`DialogManager`、`ToastManager`、Builder、`ThemeWatcher`、各类 Assist、Converter 和 `TabControlBehaviors` 是配套
API，不计作额外控件。Demo 中的代码展示器、图表和示例卡片也不计入 ShadUI 库的自研控件清单。

## 官方控件覆盖缺口

下表以官方 12.1.0 主题目录和控件源码为基准，区分专属覆盖缺口与间接覆盖。

官方依据：[Simple 主题加载总表][official-simple]、[Fluent 主题目录][official-fluent]、[控件源码目录][official-controls]
。12.1.0 与 12.1.3 的 Fluent `Controls` 目录文件名清单相同；这只说明目录项目一致，不表示两个版本实现完全相同。

### 尚无 ShadUI 专属全局主题

每行的“官方回退”均由官方总表中存在对应主题、ShadUI 加载 SimpleTheme、ShadUI 全局主题/选择器清单没有对应定义三项共同判定。子部件或资源可能受
ShadUI 影响，因此不能断言整项外观完全保持官方原样。

| 官方控件                                                                          | ShadUI 覆盖状态              | 差异与容易混淆之处                                                                                                                          |
|-----------------------------------------------------------------------------------|------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------|
| `RepeatButton`                                                                    | 官方回退；局部主题           | Spinner、Slider、时间选择器中有局部命名主题，没有独立 `RepeatButton` 类型全局主题。官方有自己的 RepeatButton 主题，不自动按 Button 计覆盖。 |
| `HyperlinkButton`                                                                 | 官方回退                     | 普通 Button 的 Ghost 等变体不等于 HyperlinkButton。                                                                                         |
| `DropDownButton`                                                                  | 官方回退                     | 自研 `SimpleDropdown` 不等于原生 DropDownButton。                                                                                           |
| `SplitButton`、`ToggleSplitButton`                                                | 官方回退                     | 后者使用前者的 StyleKey；两者均无 ShadUI 专属全局主题。                                                                                     |
| `DatePicker`、`DatePickerPresenter`                                               | 官方回退                     | 已覆盖的是 `CalendarDatePicker`；自研 `DateInput` 也不能补上官方 DatePicker API。                                                           |
| `ProgressBar`                                                                     | 官方回退                     | 自研 Loading、BusyArea、Skeleton 不等于对 ProgressBar 的换肤。                                                                              |
| `TreeView`、`TreeViewItem`                                                        | 官方回退                     | SidebarItem 是 RadioButton 派生类，不能替代树形层级与节点交互契约。                                                                         |
| `Expander`                                                                        | 官方回退                     | Sidebar 的展开功能不等于 Expander 主题。                                                                                                    |
| `GroupBox`                                                                        | 官方回退                     | Avalonia 12 的官方控件；Card 不是它的派生类。                                                                                               |
| `GridSplitter`                                                                    | 官方回退                     | Separator 是分隔线，不是可拖动的布局分隔器。                                                                                                |
| `SplitView`                                                                       | 官方回退                     | 自研 Sidebar 没有给原生 SplitView 换肤。                                                                                                    |
| `Carousel`                                                                        | 官方回退                     | 未见专属模板。                                                                                                                              |
| `TransitioningContentControl`                                                     | 官方回退                     | 自定义动画工具不代表该控件已适配。                                                                                                          |
| `TabStrip`、`TabStripItem`                                                        | 官方回退                     | TabControl/TabItem 已换肤，但它们是另一组类型。                                                                                             |
| `PipsPager`                                                                       | 官方回退                     | Avalonia 12 的分页指示器，没有对应专属主题。                                                                                                |
| `ContextMenu`                                                                     | 容器官方回退，菜单项间接覆盖 | `MenuItem` 已换肤，但 ContextMenu 外层未重写，属于部分视觉覆盖。                                                                            |
| `Flyout` / `FlyoutPresenter`                                                      | 官方回退                     | 普通内容弹层与已适配的 `MenuFlyoutPresenter` 是不同展示器。                                                                                 |
| `CommandBar`、`CommandBarButton`、`CommandBarToggleButton`、`CommandBarSeparator` | 官方回退                     | Avalonia 12 的命令栏系列；不能仅因 Button/ToggleButton 已换肤就计为整组适配。                                                               |
| `ContentPage`                                                                     | 官方回退                     | 官方页面组件，没有专属主题。                                                                                                                |
| `CarouselPage`                                                                    | 官方回退                     | 官方页面组件，不等于普通 Carousel。                                                                                                         |
| `DrawerPage`                                                                      | 官方回退                     | 官方抽屉页面，不等于 ShadUI.Sidebar。                                                                                                       |
| `NavigationPage`                                                                  | 官方回退                     | 官方导航页面，不等于 Demo 自己实现的页面切换。                                                                                              |
| `TabbedPage`                                                                      | 官方回退                     | 官方页签页面，不等于 TabControl。                                                                                                           |
| `TableView`、`TableViewCell`、`TableViewColumnHeader`、`TableViewRow`             | 官方回退                     | Avalonia 12 核心表格系列；ShadUI 的 Data Table 使用独立包 DataGrid，不能算作 TableView 覆盖。                                               |
| `RefreshContainer`、`RefreshVisualizer`                                           | 官方回退                     | 下拉刷新容器及提示器未适配。                                                                                                                |
| `NotificationCard`、`WindowNotificationManager`                                   | 官方回退                     | ShadUI 自有 Toast/ToastManager；未适配这两个官方类型。                                                                                      |
| `Avalonia.Controls.Window`、`WindowDrawnDecorations`                              | 官方回退；有自定义派生方案   | 要用 `ShadUI.Window` 获得其窗口模板，原生 Window 本身没有被该主题键替换。                                                                   |
| `ItemsControl`                                                                    | 官方回退                     | 基础项目容器没有独立 ShadUI 主题；自研派生控件各自有主题。不是列表功能完全缺失。                                                            |
| `HeaderedContentControl`                                                          | 官方回退                     | 官方有独立主题；它是带标题的基础内容容器，应与终端组件缺口分开排期。                                                                        |
| `PathIcon`                                                                        | 官方回退                     | 图标几何被大量使用，前景或大小可在局部设置，但没有独立全局 ShadUI 主题。                                                                    |

其中对一般主题库最直接的补齐候选是 `ProgressBar`、`TreeView`、`Expander`、`DatePicker`、`GridSplitter`、`SplitView`、普通
`Flyout` 和 `ContextMenu` 外层；若目标是 Avalonia 12 的完整交互控件主题覆盖，还要纳入 CommandBar、页面系列与
TableView。此处只列规划候选，没有授权或实施这些控件。

### 间接覆盖、配套部件与扩展包

| 官方类型/组件                             | 分类                   | 结论与依据                                                                                                                                    |
|-------------------------------------------|------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------|
| `MaskedTextBox`                           | 原生间接覆盖           | 官方源码明确 `StyleKeyOverride => typeof(TextBox)`，因此随 ShadUI TextBox 使用相同主题；不能因缺少 MaskedTextBox.axaml 就判漏。[源码][masked] |
| `MenuFlyout`                              | 通过展示器覆盖         | ShadUI 已重写 `MenuFlyoutPresenter` 与 `MenuItem`；不需要同名自定义 MenuFlyout 类来证明视觉接入。                                             |
| `NativeMenuBar`                           | 应用内呈现器间接覆盖   | 官方内部 `NativeMenuBarPresenter` 使用 Menu 样式键，可随 Menu 主题受益；系统原生菜单另按平台处理。[源码][nativemenubar]                       |
| `DateTimePickerPanel`、`MenuScrollViewer` | 官方配套实现           | 上游主题/父控件内的配套实现，未提供各自的 ShadUI 全局类型主题；不能把仅在模板中使用计作专属重写。                                             |
| `Thumb`、`Track`                          | 局部部件               | 滑块、滚动条等模板中使用；Thumb 存在局部模板，不能外推为任意独立 Thumb 的全局样式覆盖。                                                       |
| `TextSelectionHandle`                     | 官方回退部件           | 文本选择手柄没有 ShadUI 全局主题，应在后续文本输入交互验收时一并检查。                                                                        |
| `DataGrid`                                | 官方独立包，已直接换肤 | 详见前表。[官方默认主题][official-datagrid]                                                                                                   |
| `ColorPicker` 系列                        | 官方独立包，已直接换肤 | 详见前表。[官方主题目录][official-color]                                                                                                      |
| `TreeDataGrid`                            | 官方扩展，未集成       | ShadUI.csproj 无依赖与主题。旧开源仓库已归档，后续转入 Avalonia Accelerate；不混入核心默认控件覆盖率。[官方说明][official-treedatagrid]       |
| `ItemsRepeater`                           | 历史官方扩展，未集成   | 独立官方仓库已归档，不是当前核心默认控件的必覆盖项。[官方仓库][official-itemsrepeater]                                                        |

### 不应按“缺少独立主题文件”判漏的基础设施

| 类别               | 代表类型                                                                                                                                                                                      | 对照方式                                                                                                                                                |
|--------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------|
| 布局               | `Panel`、`Canvas`、`Grid`、`StackPanel`、`DockPanel`、`WrapPanel`、`RelativePanel`、`UniformGrid`、`VirtualizingStackPanel`、`VirtualizingCarouselPanel`、`Viewbox`、`LayoutTransformControl` | 通常直接复用原生布局能力；不存在必须给每个布局面板重写皮肤的要求。ShadUI 的断点控件是额外扩展。                                                         |
| 绘制与装饰         | `Border`、`Decorator`、`Image`、`AccessText`、`Line`、`Rectangle`、`Ellipse`、`Path`、`Polygon`、`Polyline`、`Arc`、`Sector`、`TickBar`                                                       | 由使用处的内容、画笔、尺寸等决定表现，不按专属模板数量衡量覆盖。TextBlock 也属于直接绘制类型，但 ShadUI 确实为其提供了排版主题，已在前表列出。          |
| 基类与宿主         | `Control`、`TemplatedControl`、`ContentControl`、`UserControl`、`TopLevel`、`TopLevelHost`、`NativeControlHost`、`HeaderedItemsControl`、`HeaderedSelectingItemsControl`、各 Presenter        | 属于结构或扩展基础；复用不等于自研，也不应机械加入终端组件缺口。                                                                                        |
| 主题与弹层基础设施 | `ThemeVariantScope`、`Popup`、`PopupRoot`、`OverlayPopupHost`、`EmbeddableControlRoot`、`AdornerLayer`、`PageNavigationHost`                                                                  | 使用官方基础设施或基础主题；PageNavigationHost 使用 ContentControl 样式键。应核对集成效果，不要求逐个复制模板。                                         |
| 系统集成           | `NativeMenu`、`NativeMenuItem`、`TrayIcon`、`NativeDock`、原生文件选择器/StorageProvider                                                                                                      | 由平台和原生 API 决定；应用内 DialogHost 不等于这些系统交互的替代实现。官方 `ManagedFileChooser` 是另一种托管文件选择器，不应与系统原生对话框混为一谈。 |

因此，本文不把“所有 Avalonia 公共类”当作主题库应重写的组件集合，也不使用包含布局、基类和内部部件的分母计算覆盖率。

## 验证边界

本次只建立组件目录与源码对应关系：核对项目依赖、主题加载入口、全局主题键、样式选择器和控件继承关系。没有运行上游或本仓库的构建、自动化测试与
Demo；文档变更不需要构建。运行时兼容性、明暗主题实际观感、无障碍、键盘交互及各平台行为均未据此验收。

[project]: https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/ShadUI.csproj
[resources]: https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Resources.axaml
[styles]: https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Styles.axaml
[official-simple]: https://github.com/AvaloniaUI/Avalonia/blob/12.1.0/src/Avalonia.Themes.Simple/Controls/SimpleControls.xaml
[official-fluent]: https://github.com/AvaloniaUI/Avalonia/tree/12.1.0/src/Avalonia.Themes.Fluent/Controls
[official-controls]: https://github.com/AvaloniaUI/Avalonia/tree/12.1.0/src/Avalonia.Controls
[masked]: https://github.com/AvaloniaUI/Avalonia/blob/12.1.0/src/Avalonia.Controls/MaskedTextBox.cs
[nativemenubar]: https://github.com/AvaloniaUI/Avalonia/blob/12.1.0/src/Avalonia.Controls/NativeMenuBar.cs
[official-datagrid]: https://github.com/AvaloniaUI/Avalonia.Controls.DataGrid/blob/12.1.0/src/Avalonia.Controls.DataGrid/Themes/Simple.xaml
[official-color]: https://github.com/AvaloniaUI/Avalonia/tree/12.1.0/src/Avalonia.Controls.ColorPicker/Themes/Fluent
[official-treedatagrid]: https://github.com/AvaloniaUI/Avalonia.Controls.TreeDataGrid/blob/master/readme.md
[official-itemsrepeater]: https://github.com/AvaloniaUI/Avalonia.Controls.ItemsRepeater
[button]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Button
[toggle]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/ToggleButton
[switch]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Switch
[checkbox]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/CheckBox
[radio]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/RadioButton
[textbox]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/TextBox
[autocomplete]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/AutoCompleteBox
[numeric]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/NumericUpDown
[combo]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/ComboBox
[calendar]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Calendar
[calendardate]: https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Calendar/CalendarDatePicker.axaml
[timepicker]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/TimePicker
[slider]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Slider
[listbox]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/ListBox
[tabs]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/TabControl
[menu]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Menu
[menuflyout]: https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Menu/MenuFlyoutPresenter.axaml
[tooltip]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/ToolTip
[textblock]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/TextBlock
[selectable]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/SelectableTextBlock
[label]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Label
[validation]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/DataValidation
[scroll]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/ScrollViewer
[separator]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Separator
[datagrid]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/DataGrid
[color]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/ColorPicker
[window]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Window
[sidebar]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Sidebar
[breakpoint]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Breakpoint
[avatar]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Avatar
[badge]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Badge
[card]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Card
[busy]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/BusyArea
[loading]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Loading
[skeleton]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Skeleton
[dateinput]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/DateInput
[timeinput]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/TimeInput
[dropdown]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/SimpleDropdown
[dialog]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Dialog
[toast]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Toast
