# ShadUI 与 shadcn/ui 组件对照

> 调研快照：2026-10-07。本文记录源码与官方文档的对应关系，不代表 Vhilz 的实现清单，也不代表视觉或交互已验收。

## 结论与基准

**ShadUI 是受 shadcn/ui 和 SukiUI 启发的 Avalonia 库，不是逐项完整移植版。** 其 [README][readme] 明确使用“inspired
by”；[项目文件][project] 使用 Avalonia 及官方控件包。shadcn/ui 的 React 组件与 ShadUI 的 C#/AXAML 不共享组件
API，不能将同名控件称为直接复用原版。

- ShadUI：[
  `ff1e84698d1478fc6403175858a2fd5303ad28df`](https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df)
  ，与上一篇 [Avalonia 官方控件对照](shadui-component-coverage-research.md) 使用相同版本。
-
shadcn/ui：用户提供的 [安装入口](https://ui.shadcn.com/docs/installation)、[当前组件目录](https://ui.shadcn.com/docs/components)
，并以官方源码 [`17e1129c9b9de15f78cdd79d69777082821da428`][cn-source] 核对目录。
- 当前官方文档有 Base UI、Radix UI、React Aria 三套组件页面。Base 64 项、Radix 65 项、Aria 63 项；按组件 slug 去重后共 **65
  项**。本表逐项列出这 65 项，不把三套实现重复统计。
- 当前 Base `Toast` 是有效组件；Radix 的旧 `Toast` 页面提示迁移到 `Sonner`。不能笼统说所有 Toast 都已废弃，也不能把 ShadUI
  的自有 ToastManager 说成使用了 Sonner。
- 本表不纳入 Blocks 模板、图表示例数量、安装器、主题工具和 Forms/RTL 指南。`Chart`、`Direction` 本身在正式目录中，仍分别计一项。

按下述口径逐行统计： **29 项有功能对应、12 项部分对应、24 项未提供专门实现**。这是 65 个官方目录条目的映射结果，不是
API/视觉兼容率；多个原版条目可能映射到同一个 ShadUI 实现，例如 Toast 与 Sonner 都关联它的自有通知体系。

## 阅读口径

| 状态       | 含义                                                                                                    |
|------------|---------------------------------------------------------------------------------------------------------|
| 有功能对应 | ShadUI 有实现主要用途的控件、主题或明确组合示例；不承诺原版全部子组件、变体、无障碍和交互契约一致。     |
| 部分对应   | 有相关基础能力、局部样式或 Demo 示例，但缺少同层次的封装，或需要另做组合与适配。                        |
| 未提供     | ShadUI 库未提供专门对应的组件/主题。Avalonia 可能有可用基件，应用也可能自己组合，这不算 ShadUI 已实现。 |

“原生”在下表指 **Avalonia 原生控件换肤**；“自研”指 **ShadUI 定义的自有类型/组合**。两者都不是 React
组件直接移植。缺失结论核对了 [Controls 源码目录][src]、[资源加载清单][resources]、[样式加载清单][styles] 和公开控件类，而不是只搜索
Demo 页面标题。

## 完整组件对照表

| shadcn/ui 组件                          | ShadUI 对应实现                                       | 实现性质                    | 状态与边界                                                                                                                                                                    |
|-----------------------------------------|-------------------------------------------------------|-----------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| [Accordion][cn-accordion]               | 无                                                    | —                           | **未提供**。原生 Expander 或若干折叠面板可作基础，但没有手风琴组封装与专属主题。                                                                                              |
| [Alert][cn-alert]                       | 无                                                    | —                           | **未提供**。Alert 是页面内提示块；Toast 和确认对话框不是它的对应类型。                                                                                                        |
| [Alert Dialog][cn-alert-dialog]         | DialogManager、SimpleDialogBuilder、内部 SimpleDialog | 自研                        | **有功能对应**。有标题、消息、主次操作和取消按钮，DialogOptions 控制是否允许其他方式关闭；不是独立 AlertDialog 类。[源码][dialog]                                             |
| [Aspect Ratio][cn-aspect-ratio]         | 无                                                    | —                           | **未提供**。Viewbox 缩放不等于按指定比例约束容器尺寸。                                                                                                                        |
| [Attachment][cn-attachment]             | 无                                                    | —                           | **未提供**。无附件媒体、元数据、上传状态与动作的一体化组件。                                                                                                                  |
| [Avatar][cn-avatar]                     | Avatar                                                | 自研                        | **有功能对应**。有 Source 与 Fallback；不据此推断原版头像组等全部组合都已实现。[源码][avatar]                                                                                 |
| [Badge][cn-badge]                       | Badge                                                 | 自研                        | **有功能对应**。徽标与样式变体。[源码][badge]                                                                                                                                 |
| [Breadcrumb][cn-breadcrumb]             | 无                                                    | —                           | **未提供**。无面包屑导航组件。                                                                                                                                                |
| [Bubble][cn-bubble]                     | 无                                                    | —                           | **未提供**。无对话气泡、分组、反应与折叠内容的专门组件。                                                                                                                      |
| [Button][cn-button]                     | Button、ButtonAssist                                  | 原生＋附加行为              | **有功能对应**。按钮变体、图标和忙碌表现；变体名与 API 不要求相同。[源码][button]                                                                                             |
| [Button Group][cn-button-group]         | 可排列多个 Button                                     | 原生组合基础                | **部分对应**。有按钮基件，没有对应的连接外观、分隔及方向封装。StackPanel 排列不等于完整 ButtonGroup。                                                                         |
| [Calendar][cn-calendar]                 | Calendar 及其部件                                     | 原生                        | **有功能对应**。主题包含 Calendar、CalendarItem、日期按钮；支持范围的基础来自 Avalonia，不是 React DayPicker。[源码][calendar]                                                |
| [Card][cn-card]                         | Card、CardTitle、CardDescription                      | 自研                        | **有功能对应**。Card 提供 Header、Footer 和内容区域，组合 API 与原版不同。[源码][card]                                                                                        |
| [Carousel][cn-carousel]                 | 无专属实现                                            | —                           | **未提供**。原生 Carousel 可回退 SimpleTheme，但不是 ShadUI 的轮播组件实现。                                                                                                  |
| [Chart][cn-chart]                       | Demo 的 LiveCharts                                    | 第三方 Demo                 | **部分对应（仅 Demo）**。图表依赖位于 Demo 项目，不在 ShadUI 库中；没有移植 shadcn/ui 的图表封装。[依赖][demo-project]                                                        |
| [Checkbox][cn-checkbox]                 | CheckBox                                              | 原生                        | **有功能对应**。[源码][checkbox]                                                                                                                                              |
| [Collapsible][cn-collapsible]           | 无通用控件                                            | —                           | **未提供**。Sidebar 的折叠属性只服务侧栏，不等于通用 Collapsible。                                                                                                            |
| [Combobox][cn-combobox]                 | AutoCompleteBox；另有 ComboBox                        | 原生                        | **部分对应**。AutoCompleteBox 有过滤和建议列表 Demo；ShadUI 的普通 ComboBox 更接近 Select。没有证明原版可组合的多选等模式已统一覆盖。[源码][autocomplete]、[示例][input-demo] |
| [Command][cn-command]                   | 无                                                    | —                           | **未提供**。这里是可搜索命令列表/命令面板，不是 .NET ICommand，也不是 Avalonia CommandBar。                                                                                   |
| [Context Menu][cn-context-menu]         | 原生 ContextMenu＋已换肤 MenuItem                     | 原生，部分换肤              | **部分对应**。菜单项有 ShadUI 主题，ContextMenu 外层仍由 SimpleTheme 提供。[菜单源码][menu]                                                                                   |
| [Data Table][cn-data-table]             | DataGrid 及其部件                                     | 官方独立包换肤              | **有功能对应**。数据表格与分组示例；不是 TanStack Table 的移植，也未逐项验证原版排序、过滤、分页配方。[源码][datagrid]                                                        |
| [Date Picker][cn-date-picker]           | CalendarDatePicker；DateInput                         | 原生＋自研                  | **有功能对应**。日历弹出选择与分段日期输入；不等于原版所有日期范围、时间、自然语言示例均已覆盖。[源码][calendar]、[DateInput][dateinput]                                      |
| [Dialog][cn-dialog]                     | DialogHost、DialogManager、Builder                    | 自研                        | **有功能对应**。应用内对话框；不同于桌面 Window.ShowDialog 和 React 的组合子组件 API。[源码][dialog]                                                                          |
| [Direction][cn-direction]               | Avalonia FlowDirection                                | 框架基础能力                | **部分对应**。框架有流向属性，库未提供等价 Direction Provider；也没有在本调研中确认所有模板 RTL 正确。                                                                        |
| [Drawer][cn-drawer]                     | 无                                                    | —                           | **未提供**。无通用抽屉以及对应拖拽/吸附行为；Sidebar 不是 Drawer。                                                                                                            |
| [Dropdown Menu][cn-dropdown-menu]       | Menu/MenuItem/MenuFlyoutPresenter、SimpleDropdown     | 原生＋自研                  | **有功能对应**。有菜单弹出体系及轻量下拉组件；不能将轻量 SimpleDropdown 单独视为原版所有菜单行为的等价替代。[源码][menu]、[SimpleDropdown][dropdown]                          |
| [Empty][cn-empty]                       | 无                                                    | —                           | **未提供**。无标准空状态容器、说明与动作布局。                                                                                                                                |
| [Field][cn-field]                       | ControlAssist.Label/Hint、DataValidationErrors        | 原生＋附加行为              | **部分对应**。有标签、提示、错误展示和表单验证示例，但无 Field/FieldGroup/FieldSet 这一层的组合封装。[Assist][assists]、[校验主题][validation]                                |
| [Hover Card][cn-hover-card]             | 无                                                    | —                           | **未提供**。ToolTip 不能据名称相近就当作 HoverCard 的交互内容弹层。                                                                                                           |
| [Input][cn-input]                       | TextBox、TextBoxAssist                                | 原生＋附加行为              | **有功能对应**。文本输入、清除、标签等。[源码][textbox]                                                                                                                       |
| [Input Group][cn-input-group]           | TextBox 内侧内容、搜索框示例                          | 原生组合                    | **部分对应**。可加入图标等内容，但没有独立 InputGroup、Addon、Text、Button 组合 API。[示例][input-demo]                                                                       |
| [Input OTP][cn-input-otp]               | 无                                                    | —                           | **未提供**。无一次性验证码分格输入、粘贴分配及焦点管理组件。                                                                                                                  |
| [Item][cn-item]                         | ListBox/ListBoxItem 等基件                            | 原生基础                    | **部分对应**。能构建列表项，但 ListBoxItem 是选择容器，不能当成带媒体、描述、动作的通用 Item 组合封装。[源码][listbox]                                                        |
| [Kbd][cn-kbd]                           | 无                                                    | —                           | **未提供**。菜单快捷键文本不等于独立键帽组件。                                                                                                                                |
| [Label][cn-label]                       | Label；ControlAssist.Label                            | 原生＋附加行为              | **有功能对应**。标签表现通过 Avalonia 机制实现，不使用 Web 的 htmlFor 关联方式。[源码][label]                                                                                 |
| [Marker][cn-marker]                     | 无                                                    | —                           | **未提供**。无会话内状态标记、系统注释和带标签分隔行组件。                                                                                                                    |
| [Menubar][cn-menubar]                   | Menu、MenuItem                                        | 原生                        | **有功能对应**。Demo 明确提供 Menu Bar 示例；与系统 NativeMenu 是不同层次。[源码][menu]                                                                                       |
| [Message][cn-message]                   | 无                                                    | —                           | **未提供**。无包含头像、头尾内容、方向布局的会话消息组件。                                                                                                                    |
| [Message Scroller][cn-message-scroller] | SmoothScrollAssist 仅提供通用平滑滚动                 | —                           | **未提供**。没有会话锚定、流式追随、历史加载保持位置及跳转消息的专门实现。[滚动源码][scroll]                                                                                  |
| [Native Select][cn-native-select]       | 框架 ComboBox 可完成选择                              | 原生选择基础                | **部分对应**。Avalonia 的主题化 ComboBox 不是浏览器原生 HTML select；不存在逐字面对应的 Web 原生选择控件。[源码][combo]                                                       |
| [Navigation Menu][cn-navigation-menu]   | 无                                                    | —                           | **未提供**。Menu 是动作菜单，Sidebar 是侧栏；都不能自动算作网站导航内容面板组件。                                                                                             |
| [Pagination][cn-pagination]             | 无                                                    | —                           | **未提供**。没有独立页码导航封装；DataGrid 存在不意味着已带分页栏。                                                                                                           |
| [Popover][cn-popover]                   | 无通用 ShadUI Popover                                 | —                           | **未提供**。内部使用 Popup 及原生 Flyout 可用，不等于对外提供统一 Popover。窗口上的全屏 Popover 配置也不是该组件。                                                            |
| [Progress][cn-progress]                 | 无 ProgressBar 专属主题                               | —                           | **未提供**。原生 ProgressBar 回退官方主题；Loading 是忙碌动画，不能替代确定进度展示。                                                                                         |
| [Questionnaire][cn-questionnaire]       | 无                                                    | —                           | **未提供**。无多步、单选/多选、自由填写和跳过的问卷组件。                                                                                                                     |
| [Radio Group][cn-radio-group]           | RadioButton 分组                                      | 原生组合                    | **有功能对应**。通过原生 RadioButton 的分组机制组合，不提供独立 ShadUI.RadioGroup 类。[源码][radio]                                                                           |
| [Resizable][cn-resizable]               | 无专属实现                                            | —                           | **未提供**。可自行用原生 GridSplitter 组合，但 ShadUI 没有分栏尺寸管理与专属样式封装。                                                                                        |
| [Scroll Area][cn-scroll-area]           | ScrollViewer、ScrollBar、SmoothScrollAssist           | 原生＋附加行为              | **有功能对应**。横纵滚动条主题和平滑滚动；不是浏览器滚动实现。[源码][scroll]                                                                                                  |
| [Select][cn-select]                     | ComboBox、ComboBoxItem                                | 原生                        | **有功能对应**。单项下拉选择；这是 ShadUI.ComboBox 所展示功能更直接的对应项。[源码][combo]                                                                                    |
| [Separator][cn-separator]               | Separator 样式                                        | 原生                        | **有功能对应**。提供颜色、间距，基础模板复用官方主题。[源码][separator]                                                                                                       |
| [Sheet][cn-sheet]                       | 无                                                    | —                           | **未提供**。无贴边滑出的通用对话面板；Sidebar 与居中 Dialog 都不等于 Sheet。                                                                                                  |
| [Sidebar][cn-sidebar]                   | Sidebar、SidebarItem、SidebarItemLabel                | 自研，Item 继承 RadioButton | **有功能对应**。提供展开/折叠、头尾和导航项；未证明覆盖原版移动端 Sheet、全部分组/子菜单与持久化配方。[源码][sidebar]                                                         |
| [Skeleton][cn-skeleton]                 | Skeleton                                              | 自研                        | **有功能对应**。骨架占位。[源码][skeleton]                                                                                                                                    |
| [Slider][cn-slider]                     | Slider                                                | 原生                        | **有功能对应**。基础滑动数值输入；当前主题不意味着支持原版多滑块范围等全部形态。[源码][slider]                                                                                |
| [Sonner][cn-sonner]                     | ToastHost、ToastManager 等                            | 自研相似方案                | **部分对应**。有相同通知用途，但没有集成或移植 Sonner。不能将同一 Toast 系统重复计作两套独立实现。[源码][toast]                                                               |
| [Spinner][cn-spinner]                   | Loading                                               | 自研                        | **有功能对应**。加载动画；BusyArea 是额外忙碌容器。[源码][loading]                                                                                                            |
| [Switch][cn-switch]                     | ToggleSwitch                                          | 原生                        | **有功能对应**。[源码][switch]                                                                                                                                                |
| [Table][cn-table]                       | DataGrid 可实现表格展示                               | 原生数据表格                | **部分对应**。DataGrid 是数据驱动交互控件，没有对应 HTML Table/Caption/Header/Body/Cell 那套轻量结构封装。[源码][datagrid]                                                    |
| [Tabs][cn-tabs]                         | TabControl、TabItem、TabControlBehaviors              | 原生＋附加行为              | **有功能对应**。页签及选中指示动画。[源码][tabs]                                                                                                                              |
| [Textarea][cn-textarea]                 | TextBox AcceptsReturn=True                            | 原生组合                    | **有功能对应**。Input Demo 有明确 Text Area 示例，无须再定义一个 Textarea 类。[示例][input-demo]                                                                              |
| [Toast][cn-toast]                       | ToastHost、ToastManager、ToastBuilder、内部 Toast     | 自研                        | **有功能对应**。有临时通知、类型、动作与位置等自有 API；不是原版 Toast 源码复用。[源码][toast]                                                                                |
| [Toggle][cn-toggle]                     | ToggleButton                                          | 原生                        | **有功能对应**。按钮式开关。[源码][toggle]                                                                                                                                    |
| [Toggle Group][cn-toggle-group]         | 可排列 ToggleButton                                   | 原生组合基础                | **部分对应**。没有管理组内单选/多选值与交互的统一 ToggleGroup；RadioButton 分组也不是 ToggleGroup 的完整替代。                                                                |
| [Tooltip][cn-tooltip]                   | ToolTip                                               | 原生                        | **有功能对应**。[源码][tooltip]                                                                                                                                               |
| [Typography][cn-typography]             | TextBlock、SelectableTextBlock、Label 主题及 Classes  | 原生样式                    | **有功能对应**。标题、正文等排版约定，不是独立 Typography 控件。[源码][textblock]                                                                                             |

## ShadUI 额外提供的能力

以下在当前 shadcn/ui 目录中没有同名的一对一组件条目，或是 ShadUI 面向桌面框架的额外封装；不代表 Web 无法实现相同能力。

| ShadUI 能力                             | 作用与来源                                                                                                                                                                       |
|-----------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 自定义 Window                           | 桌面标题栏、窗口按钮与平台处理。[源码][window]                                                                                                                                   |
| ColorPicker / ColorView / ColorSpectrum | Avalonia 官方颜色选择控件换肤。[源码][color]                                                                                                                                     |
| NumericUpDown / ButtonSpinner           | 原生数值步进输入；不能把它与代表加载动画的 shadcn Spinner 混为一谈。[源码][numeric]                                                                                              |
| DateInput / TimeInput / TimePicker      | 分段日期、时间输入和独立时间选择器；原版 Date Picker 页面可包含组合示例，但目录没有同名独立 TimePicker 条目。[源码][dateinput]、[TimeInput][timeinput]、[TimePicker][timepicker] |
| BusyArea                                | 内容区域忙碌遮罩。[源码][busy]                                                                                                                                                   |
| BreakpointView / BreakpointViewPort     | 根据容器宽度显示不同内容的 Avalonia 断点方案。[源码][breakpoint]                                                                                                                 |
| ListBox / SelectableTextBlock           | Avalonia 列表选择与可选中文本的独立主题。[源码][listbox]、[SelectableTextBlock][selectable]                                                                                      |

## 用于 Vhilz 规划时的含义

这份表和上一篇回答的是两条不同的覆盖轴：

1. **Avalonia 覆盖轴**：原生控件是否有 Vhilz 主题、是否保留上游交互契约。
2. **shadcn/ui 覆盖轴**：产品界面所需的组合组件是否齐全，例如 Alert、Breadcrumb、Command、Popover、Sheet、Pagination、Input OTP
   等。

因此，仅补齐 Avalonia 官方控件不会自动获得完整 shadcn/ui 组件集；反过来，有自研 Sidebar/Toast 也不能视为覆盖所有 Avalonia
导航/通知类型。ShadUI 可作为普通控件换肤与部分组合控件的参考，但不适合作为“原版所有组件都已实现”的清单。

## 验证边界

本次核对官方组件目录、文档说明、ShadUI 控件类、AXAML 主题与 Demo 使用；没有运行两项目的 UI，也没有测试
API、可访问性、键盘、触摸、明暗主题或跨平台等价性。仅新增调研文档，不运行构建或自动化测试。

[cn-source]: https://github.com/shadcn-ui/ui/tree/17e1129c9b9de15f78cdd79d69777082821da428/apps/v4/content/docs/components
[readme]: https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/README.md
[src]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls
[demo-project]: https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI.Demo/ShadUI.Demo.csproj
[input-demo]: https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI.Demo/Views/InputPage.axaml
[assists]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/AttachedProperties
[project]: https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/ShadUI.csproj
[resources]: https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Resources.axaml
[styles]: https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Styles.axaml
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
[timepicker]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/TimePicker
[slider]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Slider
[listbox]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/ListBox
[tabs]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/TabControl
[menu]: https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Menu
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
[cn-accordion]: https://ui.shadcn.com/docs/components/base/accordion
[cn-alert]: https://ui.shadcn.com/docs/components/base/alert
[cn-alert-dialog]: https://ui.shadcn.com/docs/components/base/alert-dialog
[cn-aspect-ratio]: https://ui.shadcn.com/docs/components/base/aspect-ratio
[cn-attachment]: https://ui.shadcn.com/docs/components/base/attachment
[cn-avatar]: https://ui.shadcn.com/docs/components/base/avatar
[cn-badge]: https://ui.shadcn.com/docs/components/base/badge
[cn-breadcrumb]: https://ui.shadcn.com/docs/components/base/breadcrumb
[cn-bubble]: https://ui.shadcn.com/docs/components/base/bubble
[cn-button]: https://ui.shadcn.com/docs/components/base/button
[cn-button-group]: https://ui.shadcn.com/docs/components/base/button-group
[cn-calendar]: https://ui.shadcn.com/docs/components/base/calendar
[cn-card]: https://ui.shadcn.com/docs/components/base/card
[cn-carousel]: https://ui.shadcn.com/docs/components/base/carousel
[cn-chart]: https://ui.shadcn.com/docs/components/base/chart
[cn-checkbox]: https://ui.shadcn.com/docs/components/base/checkbox
[cn-collapsible]: https://ui.shadcn.com/docs/components/base/collapsible
[cn-combobox]: https://ui.shadcn.com/docs/components/base/combobox
[cn-command]: https://ui.shadcn.com/docs/components/base/command
[cn-context-menu]: https://ui.shadcn.com/docs/components/base/context-menu
[cn-data-table]: https://ui.shadcn.com/docs/components/base/data-table
[cn-date-picker]: https://ui.shadcn.com/docs/components/base/date-picker
[cn-dialog]: https://ui.shadcn.com/docs/components/base/dialog
[cn-direction]: https://ui.shadcn.com/docs/components/base/direction
[cn-drawer]: https://ui.shadcn.com/docs/components/base/drawer
[cn-dropdown-menu]: https://ui.shadcn.com/docs/components/base/dropdown-menu
[cn-empty]: https://ui.shadcn.com/docs/components/base/empty
[cn-field]: https://ui.shadcn.com/docs/components/base/field
[cn-hover-card]: https://ui.shadcn.com/docs/components/base/hover-card
[cn-input]: https://ui.shadcn.com/docs/components/base/input
[cn-input-group]: https://ui.shadcn.com/docs/components/base/input-group
[cn-input-otp]: https://ui.shadcn.com/docs/components/base/input-otp
[cn-item]: https://ui.shadcn.com/docs/components/base/item
[cn-kbd]: https://ui.shadcn.com/docs/components/base/kbd
[cn-label]: https://ui.shadcn.com/docs/components/base/label
[cn-marker]: https://ui.shadcn.com/docs/components/base/marker
[cn-menubar]: https://ui.shadcn.com/docs/components/base/menubar
[cn-message]: https://ui.shadcn.com/docs/components/base/message
[cn-message-scroller]: https://ui.shadcn.com/docs/components/base/message-scroller
[cn-native-select]: https://ui.shadcn.com/docs/components/base/native-select
[cn-navigation-menu]: https://ui.shadcn.com/docs/components/base/navigation-menu
[cn-pagination]: https://ui.shadcn.com/docs/components/base/pagination
[cn-popover]: https://ui.shadcn.com/docs/components/base/popover
[cn-progress]: https://ui.shadcn.com/docs/components/base/progress
[cn-questionnaire]: https://ui.shadcn.com/docs/components/base/questionnaire
[cn-radio-group]: https://ui.shadcn.com/docs/components/base/radio-group
[cn-resizable]: https://ui.shadcn.com/docs/components/base/resizable
[cn-scroll-area]: https://ui.shadcn.com/docs/components/base/scroll-area
[cn-select]: https://ui.shadcn.com/docs/components/base/select
[cn-separator]: https://ui.shadcn.com/docs/components/base/separator
[cn-sheet]: https://ui.shadcn.com/docs/components/base/sheet
[cn-sidebar]: https://ui.shadcn.com/docs/components/base/sidebar
[cn-skeleton]: https://ui.shadcn.com/docs/components/base/skeleton
[cn-slider]: https://ui.shadcn.com/docs/components/base/slider
[cn-sonner]: https://ui.shadcn.com/docs/components/radix/sonner
[cn-spinner]: https://ui.shadcn.com/docs/components/base/spinner
[cn-switch]: https://ui.shadcn.com/docs/components/base/switch
[cn-table]: https://ui.shadcn.com/docs/components/base/table
[cn-tabs]: https://ui.shadcn.com/docs/components/base/tabs
[cn-textarea]: https://ui.shadcn.com/docs/components/base/textarea
[cn-toast]: https://ui.shadcn.com/docs/components/base/toast
[cn-toggle]: https://ui.shadcn.com/docs/components/base/toggle
[cn-toggle-group]: https://ui.shadcn.com/docs/components/base/toggle-group
[cn-tooltip]: https://ui.shadcn.com/docs/components/base/tooltip
[cn-typography]: https://ui.shadcn.com/docs/components/base/typography
