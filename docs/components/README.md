# 组件建设与领取表

按 **shadcn/ui 的组件目录组织需求，先给 Avalonia 原生控件做主题，再补组合或自研能力**。这是用户已确认的顺序。视觉仍遵循本仓库的
Linear/克制玻璃方向，不要求一比一复制 shadcn 外观或 React API。

本计划有 **65 个 shadcn 目录映射、57 张主线组件/支撑任务卡、15 张 Avalonia 补齐或可选包任务卡，以及 2 张串行前置卡**
。任务数量不等于组件数量：共享同一原生类型的需求合并，内部部件跟随主控件归属。本轮已实施原生首轮外观与 Demo 宿主，实际范围和验证见
[原生外观交付](delivery/native-appearance.md)，各卡状态以领取记录为准。

## 从哪里开始

样式编写入口分为 [AI 选择器与 Classes 执行规范](selector-class-rules.md) 和 [人类指南与语法速查](selector-class-guide.md)，分别用于执行约束和日常查阅。

1. 读 [AGENTS.md](../../AGENTS.md) 和 [执行约定](work-contract.md)。
2. [F00](delivery/F00.md) 与 [F01](delivery/F01.md) 首轮已集成：交接当前未提交基线、接入独立 Sample 目录和共享资源。
   Demo 主窗口的“打开原生组件外观目录”进入明暗主题验收入口。
3. 其他负责人可以先领取组件卡读上游源码和准备方案；编码以依赖与资源契约可用为前提。核对 [领取记录](claims.md)，确认任务没有被别人占用。
4. 选择下表中的条目，打开任务卡即可看到范围、硬依赖、独占文件、Demo
   和完成条件。复制 [领取指令](work-contract.md#6-可直接复制的领取指令) 开工。
5. 使用 [交付模板](delivery-template.md) 形成接入包。组件负责人只改自己的文件；集成人顺序改 Token、ResourceKeys、生产主题入口和
   Demo 导航。构建通过后标记视觉待验收，用户确认后才算该范围已验收。

**不要让多个执行者同时在当前工作目录改代码。** 独立工作树隔离文件，明确所有权隔离主题/资源语义，单一集成人隔离共享入口。表格本身不具备自动互斥能力。

## 当前基线

当前库已有窗口/标题栏/图标与原生控件首轮主题，F00/F01 已接入独立目录与共享资源，N01 接续原有 Button 工作。41 个 Sample 使用正式主题，工具区局部使用 Fluent。库不自动加载 Fluent/Simple 回退，未提交工作保留；资源、依赖与命名空间以当前源码为准。

历史研究入口：[ShadUI 对 Avalonia](../shadui-component-coverage-research.md)、[ShadUI 对 shadcn/ui](../shadui-shadcn-component-comparison.md)。
**ShadUI 有某组件，不表示本库已经实现它**；本表以本库任务状态为准。

## 推进阶段与并行组合

| 阶段 | 内容                                               | 开始条件                                                    |
|------|----------------------------------------------------|-------------------------------------------------------------|
| P0   | F00 基线与 Demo、F01 共享资源                      | 先解决共享入口与未提交工作归属；两卡由集成人协调。          |
| P1   | 普通原生控件、输入/滚动等基础                      | F00 与 F01 首轮可用，按各卡硬依赖领取；N01 先交接现有工作。 |
| P2   | 组合度较高的原生选择、弹层、菜单、日期、表格与用法 | 相关 P1 入口已集成；可以在稳定依赖上并行。                  |
| P2A  | shadcn 目录之外的 Avalonia 补齐项                  | 仍走原生优先；与 P2 按需求并行，新增包项需单独确认。        |
| P3   | Alert、Card、Command、Sheet 等组合/自研组件        | 对应原生基件稳定；先确定最小公开用法和必要 API 决策。       |

首批可并行准备 N02 文字、N03 分隔线、N05 复选框、N07 开关；接入资源仍串行。N08 滚动完成后解锁 N09 输入和 N14 列表，随后解锁
N15/N16。不要让“所有 P1 都必须结束”成为不必要的大锁；以任务卡的硬依赖为准。同一个模板家族不拆给多人同时改。

优先顺序是排期方向，不是架构依赖；没有硬依赖的 P3 项可以提前做设计，但不因此跳过用户确定的原生覆盖优先级。已有窗口/图标/GrassButton
和平台模块不在普通控件任务中顺手改动。

## shadcn/ui 逐项归属

基准为 [官方目录](https://ui.shadcn.com/docs/components) 与调研时固定源码中的 65 个去重目录项；不把 Base/Radix/Aria
重复计数。每项只有一个主责卡，同一主责卡可包含多个明确里程碑。卡片未完成的后续里程碑不算该行已覆盖。

| shadcn/ui 条目                                                                  | 主责任务                                                   | 首轮实现方向                                   |
|---------------------------------------------------------------------------------|------------------------------------------------------------|------------------------------------------------|
| [Accordion](https://ui.shadcn.com/docs/components/base/accordion)               | [C05 · Accordion](tasks/C05.md)                            | 先确定组合/扩展契约，再按卡片实施              |
| [Alert](https://ui.shadcn.com/docs/components/base/alert)                       | [C01 · Alert 提示块](tasks/C01.md)                         | 先确定组合/扩展契约，再按卡片实施              |
| [Alert Dialog](https://ui.shadcn.com/docs/components/base/alert-dialog)         | [N27 · Dialog 与 AlertDialog 的原生窗口方案](tasks/N27.md) | 复用同一模态用法，单列确认/取消验收            |
| [Aspect Ratio](https://ui.shadcn.com/docs/components/base/aspect-ratio)         | [C06 · AspectRatio](tasks/C06.md)                          | 先核对原生布局能力，必要时小型容器             |
| [Attachment](https://ui.shadcn.com/docs/components/base/attachment)             | [C24 · Attachment](tasks/C24.md)                           | 先确定组合/扩展契约，再按卡片实施              |
| [Avatar](https://ui.shadcn.com/docs/components/base/avatar)                     | [C02 · Avatar](tasks/C02.md)                               | 先确定组合/扩展契约，再按卡片实施              |
| [Badge](https://ui.shadcn.com/docs/components/base/badge)                       | [C03 · Badge](tasks/C03.md)                                | 先确定组合/扩展契约，再按卡片实施              |
| [Breadcrumb](https://ui.shadcn.com/docs/components/base/breadcrumb)             | [C07 · Breadcrumb](tasks/C07.md)                           | 先确定组合/扩展契约，再按卡片实施              |
| [Bubble](https://ui.shadcn.com/docs/components/base/bubble)                     | [C25 · Bubble](tasks/C25.md)                               | 先确定组合/扩展契约，再按卡片实施              |
| [Button](https://ui.shadcn.com/docs/components/base/button)                     | [N01 · Button 与 RepeatButton](tasks/N01.md)               | 接续现有 Button；不改窗口按钮                  |
| [Button Group](https://ui.shadcn.com/docs/components/base/button-group)         | [C08 · ButtonGroup](tasks/C08.md)                          | 先确定组合/扩展契约，再按卡片实施              |
| [Calendar](https://ui.shadcn.com/docs/components/base/calendar)                 | [N21 · Calendar](tasks/N21.md)                             | Calendar 与全部配套部件同一所有者              |
| [Card](https://ui.shadcn.com/docs/components/base/card)                         | [C04 · Card](tasks/C04.md)                                 | 先确定组合/扩展契约，再按卡片实施              |
| [Carousel](https://ui.shadcn.com/docs/components/base/carousel)                 | [N23 · Carousel](tasks/N23.md)                             | 原生控件主题或明确原生用法                     |
| [Chart](https://ui.shadcn.com/docs/components/base/chart)                       | [C09 · Chart](tasks/C09.md)                                | 先讨论依赖；一种图表最小实现                   |
| [Checkbox](https://ui.shadcn.com/docs/components/base/checkbox)                 | [N05 · CheckBox](tasks/N05.md)                             | 原生控件主题或明确原生用法                     |
| [Collapsible](https://ui.shadcn.com/docs/components/base/collapsible)           | [N12 · Expander](tasks/N12.md)                             | 原生 Expander；Accordion 组由 C05 负责         |
| [Combobox](https://ui.shadcn.com/docs/components/base/combobox)                 | [N16 · AutoCompleteBox](tasks/N16.md)                      | 原生 AutoCompleteBox；多选标签另列迭代         |
| [Command](https://ui.shadcn.com/docs/components/base/command)                   | [C10 · Command 搜索命令面板](tasks/C10.md)                 | 先确定组合/扩展契约，再按卡片实施              |
| [Context Menu](https://ui.shadcn.com/docs/components/base/context-menu)         | [N20 · 菜单家族](tasks/N20.md)                             | 菜单家族统一拥有外层与 MenuItem                |
| [Data Table](https://ui.shadcn.com/docs/components/base/data-table)             | [N26 · TableView 与表格场景](tasks/N26.md)                 | N26 后续数据交互里程碑；按 TableView 实际 API  |
| [Date Picker](https://ui.shadcn.com/docs/components/base/date-picker)           | [N22 · CalendarDatePicker 与 DatePicker](tasks/N22.md)     | CalendarDatePicker/DatePicker 分里程碑         |
| [Dialog](https://ui.shadcn.com/docs/components/base/dialog)                     | [N27 · Dialog 与 AlertDialog 的原生窗口方案](tasks/N27.md) | 首轮为现有 VhilzWindow 原生模态用法            |
| [Direction](https://ui.shadcn.com/docs/components/base/direction)               | [N28 · Direction 与原生流向](tasks/N28.md)                 | FlowDirection 用法与方向审计，不新建 Provider  |
| [Drawer](https://ui.shadcn.com/docs/components/base/drawer)                     | [C19 · Sheet 与 Drawer](tasks/C19.md)                      | C19 后续手势/吸附里程碑，需单独确认            |
| [Dropdown Menu](https://ui.shadcn.com/docs/components/base/dropdown-menu)       | [N20 · 菜单家族](tasks/N20.md)                             | 原生 MenuFlyout/菜单；不另造轻量管理器         |
| [Empty](https://ui.shadcn.com/docs/components/base/empty)                       | [C23 · Empty](tasks/C23.md)                                | 先确定组合/扩展契约，再按卡片实施              |
| [Field](https://ui.shadcn.com/docs/components/base/field)                       | [C11 · Field 表单布局](tasks/C11.md)                       | 先确定组合/扩展契约，再按卡片实施              |
| [Hover Card](https://ui.shadcn.com/docs/components/base/hover-card)             | [C12 · HoverCard](tasks/C12.md)                            | 先确定组合/扩展契约，再按卡片实施              |
| [Input](https://ui.shadcn.com/docs/components/base/input)                       | [N09 · TextBox、MaskedTextBox 与校验提示](tasks/N09.md)    | TextBox；同时接入 MaskedTextBox 与校验提示     |
| [Input Group](https://ui.shadcn.com/docs/components/base/input-group)           | [C13 · InputGroup](tasks/C13.md)                           | 先确定组合/扩展契约，再按卡片实施              |
| [Input OTP](https://ui.shadcn.com/docs/components/base/input-otp)               | [C14 · InputOTP](tasks/C14.md)                             | 先确定组合/扩展契约，再按卡片实施              |
| [Item](https://ui.shadcn.com/docs/components/base/item)                         | [C15 · Item 内容项](tasks/C15.md)                          | 先确定组合/扩展契约，再按卡片实施              |
| [Kbd](https://ui.shadcn.com/docs/components/base/kbd)                           | [C16 · Kbd](tasks/C16.md)                                  | 先确定组合/扩展契约，再按卡片实施              |
| [Label](https://ui.shadcn.com/docs/components/base/label)                       | [N02 · 文字与标签](tasks/N02.md)                           | 原生控件主题或明确原生用法                     |
| [Marker](https://ui.shadcn.com/docs/components/base/marker)                     | [C26 · Marker](tasks/C26.md)                               | 先确定组合/扩展契约，再按卡片实施              |
| [Menubar](https://ui.shadcn.com/docs/components/base/menubar)                   | [N20 · 菜单家族](tasks/N20.md)                             | 原生 Menu/MenuItem                             |
| [Message](https://ui.shadcn.com/docs/components/base/message)                   | [C27 · Message](tasks/C27.md)                              | 先确定组合/扩展契约，再按卡片实施              |
| [Message Scroller](https://ui.shadcn.com/docs/components/base/message-scroller) | [C28 · MessageScroller](tasks/C28.md)                      | 先确定组合/扩展契约，再按卡片实施              |
| [Native Select](https://ui.shadcn.com/docs/components/base/native-select)       | [N15 · ComboBox 与选择下拉](tasks/N15.md)                  | 记录桌面 ComboBox 用途对应，不模拟 HTML select |
| [Navigation Menu](https://ui.shadcn.com/docs/components/base/navigation-menu)   | [C17 · NavigationMenu](tasks/C17.md)                       | 先确定组合/扩展契约，再按卡片实施              |
| [Pagination](https://ui.shadcn.com/docs/components/base/pagination)             | [C18 · Pagination](tasks/C18.md)                           | 先确定组合/扩展契约，再按卡片实施              |
| [Popover](https://ui.shadcn.com/docs/components/base/popover)                   | [N19 · Flyout 与 Popover](tasks/N19.md)                    | 原生控件主题或明确原生用法                     |
| [Progress](https://ui.shadcn.com/docs/components/base/progress)                 | [N11 · ProgressBar 与忙碌展示](tasks/N11.md)               | 原生 ProgressBar 确定/不确定进度               |
| [Questionnaire](https://ui.shadcn.com/docs/components/base/questionnaire)       | [C29 · Questionnaire](tasks/C29.md)                        | 先确定组合/扩展契约，再按卡片实施              |
| [Radio Group](https://ui.shadcn.com/docs/components/base/radio-group)           | [N06 · RadioButton 与分组示例](tasks/N06.md)               | 原生控件主题或明确原生用法                     |
| [Resizable](https://ui.shadcn.com/docs/components/base/resizable)               | [N13 · GridSplitter 与分栏示例](tasks/N13.md)              | 原生 GridSplitter 与分栏组合                   |
| [Scroll Area](https://ui.shadcn.com/docs/components/base/scroll-area)           | [N08 · ScrollViewer 与 ScrollBar](tasks/N08.md)            | 原生控件主题或明确原生用法                     |
| [Select](https://ui.shadcn.com/docs/components/base/select)                     | [N15 · ComboBox 与选择下拉](tasks/N15.md)                  | 原生 ComboBox/ComboBoxItem                     |
| [Separator](https://ui.shadcn.com/docs/components/base/separator)               | [N03 · Separator](tasks/N03.md)                            | 原生控件主题或明确原生用法                     |
| [Sheet](https://ui.shadcn.com/docs/components/base/sheet)                       | [C19 · Sheet 与 Drawer](tasks/C19.md)                      | C19 首里程碑：统一贴边面板                     |
| [Sidebar](https://ui.shadcn.com/docs/components/base/sidebar)                   | [C20 · Sidebar](tasks/C20.md)                              | 复用 SplitView/选择基件；导航交应用            |
| [Skeleton](https://ui.shadcn.com/docs/components/base/skeleton)                 | [C21 · Skeleton](tasks/C21.md)                             | 先确定组合/扩展契约，再按卡片实施              |
| [Slider](https://ui.shadcn.com/docs/components/base/slider)                     | [N10 · Slider](tasks/N10.md)                               | 原生控件主题或明确原生用法                     |
| [Sonner](https://ui.shadcn.com/docs/components/radix/sonner)                    | [N24 · 官方通知与 Toast](tasks/N24.md)                     | 复用同一通知能力；不引入第二套服务或 Sonner 包 |
| [Spinner](https://ui.shadcn.com/docs/components/base/spinner)                   | [N11 · ProgressBar 与忙碌展示](tasks/N11.md)               | N11 的后续忙碌展示里程碑；不等同 ButtonSpinner |
| [Switch](https://ui.shadcn.com/docs/components/base/switch)                     | [N07 · ToggleSwitch](tasks/N07.md)                         | 原生控件主题或明确原生用法                     |
| [Table](https://ui.shadcn.com/docs/components/base/table)                       | [N26 · TableView 与表格场景](tasks/N26.md)                 | N26 的只读表格里程碑；明确与 HTML Table 不同   |
| [Tabs](https://ui.shadcn.com/docs/components/base/tabs)                         | [N17 · TabControl 与 TabStrip](tasks/N17.md)               | 原生控件主题或明确原生用法                     |
| [Textarea](https://ui.shadcn.com/docs/components/base/textarea)                 | [N09 · TextBox、MaskedTextBox 与校验提示](tasks/N09.md)    | 与 Input 共用 TextBox，使用原生多行能力        |
| [Toast](https://ui.shadcn.com/docs/components/base/toast)                       | [N24 · 官方通知与 Toast](tasks/N24.md)                     | 优先官方通知卡/管理器换肤                      |
| [Toggle](https://ui.shadcn.com/docs/components/base/toggle)                     | [N04 · ToggleButton](tasks/N04.md)                         | 原生控件主题或明确原生用法                     |
| [Toggle Group](https://ui.shadcn.com/docs/components/base/toggle-group)         | [C22 · ToggleGroup](tasks/C22.md)                          | 先确定组合/扩展契约，再按卡片实施              |
| [Tooltip](https://ui.shadcn.com/docs/components/base/tooltip)                   | [N18 · ToolTip](tasks/N18.md)                              | 原生控件主题或明确原生用法                     |
| [Typography](https://ui.shadcn.com/docs/components/base/typography)             | [N02 · 文字与标签](tasks/N02.md)                           | 原生控件主题或明确原生用法                     |

## 可领取任务卡索引

风险是实现与回归风险的相对判断，不是工期承诺；不代表必须使用某个测试强度。状态统一查看 [领取记录](claims.md)
。以下依赖列表省略所有普通任务共同的 F00/F01 首轮前置。

### P0 · 串行前置

| 任务卡                                       | 硬依赖       | 风险 |
|----------------------------------------------|--------------|------|
| [F00 · 工作基线与 Demo 接入](tasks/F00.md)   | 无组件硬依赖 | 中   |
| [F01 · 共享资源与视觉基线协调](tasks/F01.md) | 无组件硬依赖 | 中   |

### P1 · 基础原生控件

| 任务卡                                                  | 硬依赖              | 风险 |
|---------------------------------------------------------|---------------------|------|
| [N01 · Button 与 RepeatButton](tasks/N01.md)            | 无组件硬依赖        | 中   |
| [N02 · 文字与标签](tasks/N02.md)                        | 无组件硬依赖        | 低   |
| [N03 · Separator](tasks/N03.md)                         | 无组件硬依赖        | 低   |
| [N04 · ToggleButton](tasks/N04.md)                      | 无组件硬依赖        | 中   |
| [N05 · CheckBox](tasks/N05.md)                          | 无组件硬依赖        | 中   |
| [N06 · RadioButton 与分组示例](tasks/N06.md)            | 无组件硬依赖        | 中   |
| [N07 · ToggleSwitch](tasks/N07.md)                      | 无组件硬依赖        | 中   |
| [N08 · ScrollViewer 与 ScrollBar](tasks/N08.md)         | 无组件硬依赖        | 高   |
| [N09 · TextBox、MaskedTextBox 与校验提示](tasks/N09.md) | [N08](tasks/N08.md) | 高   |
| [N10 · Slider](tasks/N10.md)                            | 无组件硬依赖        | 中   |
| [N11 · ProgressBar 与忙碌展示](tasks/N11.md)            | 无组件硬依赖        | 中   |
| [N12 · Expander](tasks/N12.md)                          | 无组件硬依赖        | 中   |
| [N13 · GridSplitter 与分栏示例](tasks/N13.md)           | 无组件硬依赖        | 中   |

### P2 · 原生组合与复杂控件

| 任务卡                                                     | 硬依赖                                                                             | 风险 |
|------------------------------------------------------------|------------------------------------------------------------------------------------|------|
| [N14 · ItemsControl 与 ListBox](tasks/N14.md)              | [N08](tasks/N08.md)                                                                | 高   |
| [N15 · ComboBox 与选择下拉](tasks/N15.md)                  | [N08](tasks/N08.md)、[N09](tasks/N09.md)                                           | 高   |
| [N16 · AutoCompleteBox](tasks/N16.md)                      | [N08](tasks/N08.md)、[N09](tasks/N09.md)、[N14](tasks/N14.md)                      | 高   |
| [N17 · TabControl 与 TabStrip](tasks/N17.md)               | 无组件硬依赖                                                                       | 高   |
| [N18 · ToolTip](tasks/N18.md)                              | 无组件硬依赖                                                                       | 中   |
| [N19 · Flyout 与 Popover](tasks/N19.md)                    | 无组件硬依赖                                                                       | 高   |
| [N20 · 菜单家族](tasks/N20.md)                             | [N08](tasks/N08.md)、[N03](tasks/N03.md)                                           | 高   |
| [N21 · Calendar](tasks/N21.md)                             | [N01](tasks/N01.md)                                                                | 高   |
| [N22 · CalendarDatePicker 与 DatePicker](tasks/N22.md)     | [N09](tasks/N09.md)、[N19](tasks/N19.md)、[N21](tasks/N21.md)、[N08](tasks/N08.md) | 高   |
| [N23 · Carousel](tasks/N23.md)                             | [N01](tasks/N01.md)                                                                | 中   |
| [N24 · 官方通知与 Toast](tasks/N24.md)                     | [N01](tasks/N01.md)                                                                | 高   |
| [N25 · SplitView](tasks/N25.md)                            | 无组件硬依赖                                                                       | 高   |
| [N26 · TableView 与表格场景](tasks/N26.md)                 | [N08](tasks/N08.md)、[N09](tasks/N09.md)、[N05](tasks/N05.md)                      | 高   |
| [N27 · Dialog 与 AlertDialog 的原生窗口方案](tasks/N27.md) | [N01](tasks/N01.md)                                                                | 中   |
| [N28 · Direction 与原生流向](tasks/N28.md)                 | [N02](tasks/N02.md)、[N09](tasks/N09.md)、[N20](tasks/N20.md)                      | 中   |

### P2A · Avalonia 补齐与可选包

| 任务卡                                      | 硬依赖                                                                             | 风险 |
|---------------------------------------------|------------------------------------------------------------------------------------|------|
| [E01 · TreeView](tasks/E01.md)              | [N08](tasks/N08.md)、[N14](tasks/N14.md)                                           | 高   |
| [E02 · NumericUpDown](tasks/E02.md)         | [N09](tasks/N09.md)、[N01](tasks/N01.md)                                           | 高   |
| [E03 · TimePicker](tasks/E03.md)            | [N08](tasks/N08.md)、[N09](tasks/N09.md)、[N19](tasks/N19.md)                      | 高   |
| [E04 · 官方 ColorPicker](tasks/E04.md)      | [N09](tasks/N09.md)、[N10](tasks/N10.md)、[N17](tasks/N17.md)、[N19](tasks/N19.md) | 高   |
| [E05 · 其他原生按钮](tasks/E05.md)          | [N01](tasks/N01.md)、[N19](tasks/N19.md)                                           | 中   |
| [E06 · GroupBox 与带标题容器](tasks/E06.md) | [N02](tasks/N02.md)                                                                | 低   |
| [E07 · CommandBar](tasks/E07.md)            | [N01](tasks/N01.md)、[N04](tasks/N04.md)、[N03](tasks/N03.md)、[N20](tasks/N20.md) | 高   |
| [E08 · ContentPage](tasks/E08.md)           | 无组件硬依赖                                                                       | 高   |
| [E09 · NavigationPage](tasks/E09.md)        | [E08](tasks/E08.md)、[N01](tasks/N01.md)                                           | 高   |
| [E10 · TabbedPage](tasks/E10.md)            | [E08](tasks/E08.md)、[N17](tasks/N17.md)                                           | 高   |
| [E11 · DrawerPage](tasks/E11.md)            | [E08](tasks/E08.md)、[N25](tasks/N25.md)                                           | 高   |
| [E12 · CarouselPage](tasks/E12.md)          | [E08](tasks/E08.md)、[N23](tasks/N23.md)                                           | 高   |
| [E13 · PipsPager](tasks/E13.md)             | [N01](tasks/N01.md)                                                                | 中   |
| [E14 · RefreshContainer](tasks/E14.md)      | [N08](tasks/N08.md)、[N11](tasks/N11.md)                                           | 高   |
| [E15 · 可选 DataGrid 包](tasks/E15.md)      | [N08](tasks/N08.md)、[N09](tasks/N09.md)、[N05](tasks/N05.md)                      | 高   |

### P3 · 组合与自研组件

| 任务卡                                     | 硬依赖                                                                                                  | 风险 |
|--------------------------------------------|---------------------------------------------------------------------------------------------------------|------|
| [C01 · Alert 提示块](tasks/C01.md)         | [N02](tasks/N02.md)                                                                                     | 中   |
| [C02 · Avatar](tasks/C02.md)               | 无组件硬依赖                                                                                            | 低   |
| [C03 · Badge](tasks/C03.md)                | [N02](tasks/N02.md)                                                                                     | 低   |
| [C04 · Card](tasks/C04.md)                 | [N02](tasks/N02.md)                                                                                     | 低   |
| [C05 · Accordion](tasks/C05.md)            | [N12](tasks/N12.md)                                                                                     | 高   |
| [C06 · AspectRatio](tasks/C06.md)          | 无组件硬依赖                                                                                            | 中   |
| [C07 · Breadcrumb](tasks/C07.md)           | [N01](tasks/N01.md)、[N02](tasks/N02.md)                                                                | 中   |
| [C08 · ButtonGroup](tasks/C08.md)          | [N01](tasks/N01.md)、[N03](tasks/N03.md)                                                                | 中   |
| [C09 · Chart](tasks/C09.md)                | 无组件硬依赖                                                                                            | 高   |
| [C10 · Command 搜索命令面板](tasks/C10.md) | [N09](tasks/N09.md)、[N14](tasks/N14.md)、[N27](tasks/N27.md)                                           | 高   |
| [C11 · Field 表单布局](tasks/C11.md)       | [N02](tasks/N02.md)、[N09](tasks/N09.md)、[N05](tasks/N05.md)、[N06](tasks/N06.md)                      | 中   |
| [C12 · HoverCard](tasks/C12.md)            | [N19](tasks/N19.md)                                                                                     | 高   |
| [C13 · InputGroup](tasks/C13.md)           | [N09](tasks/N09.md)、[N01](tasks/N01.md)                                                                | 中   |
| [C14 · InputOTP](tasks/C14.md)             | [N09](tasks/N09.md)                                                                                     | 高   |
| [C15 · Item 内容项](tasks/C15.md)          | [N02](tasks/N02.md)、[N01](tasks/N01.md)                                                                | 中   |
| [C16 · Kbd](tasks/C16.md)                  | [N02](tasks/N02.md)                                                                                     | 低   |
| [C17 · NavigationMenu](tasks/C17.md)       | [N19](tasks/N19.md)、[N01](tasks/N01.md)                                                                | 高   |
| [C18 · Pagination](tasks/C18.md)           | [N01](tasks/N01.md)                                                                                     | 中   |
| [C19 · Sheet 与 Drawer](tasks/C19.md)      | [N25](tasks/N25.md)、[N19](tasks/N19.md)                                                                | 高   |
| [C20 · Sidebar](tasks/C20.md)              | [N25](tasks/N25.md)、[N06](tasks/N06.md)、[N08](tasks/N08.md)                                           | 高   |
| [C21 · Skeleton](tasks/C21.md)             | 无组件硬依赖                                                                                            | 低   |
| [C22 · ToggleGroup](tasks/C22.md)          | [N04](tasks/N04.md)                                                                                     | 高   |
| [C23 · Empty](tasks/C23.md)                | [N02](tasks/N02.md)、[N01](tasks/N01.md)                                                                | 低   |
| [C24 · Attachment](tasks/C24.md)           | [C15](tasks/C15.md)、[N11](tasks/N11.md)、[N01](tasks/N01.md)                                           | 高   |
| [C25 · Bubble](tasks/C25.md)               | [N02](tasks/N02.md)                                                                                     | 中   |
| [C26 · Marker](tasks/C26.md)               | [N02](tasks/N02.md)、[N03](tasks/N03.md)                                                                | 低   |
| [C27 · Message](tasks/C27.md)              | [C02](tasks/C02.md)、[C25](tasks/C25.md)、[C15](tasks/C15.md)                                           | 中   |
| [C28 · MessageScroller](tasks/C28.md)      | [N08](tasks/N08.md)、[C27](tasks/C27.md)                                                                | 高   |
| [C29 · Questionnaire](tasks/C29.md)        | [C11](tasks/C11.md)、[N05](tasks/N05.md)、[N06](tasks/N06.md)、[N09](tasks/N09.md)、[N01](tasks/N01.md) | 高   |

## 覆盖范围与后续迭代

P2A 把 TreeView、数值/时间、额外按钮、GroupBox、CommandBar、五种页面、PipsPager 和刷新控件显式排入原生补齐，防止 shadcn
目录没有相同名字就漏掉。E04 ColorPicker、E15 DataGrid 是需要依赖决策的官方独立包；不阻塞核心 Avalonia 控件推进。TreeDataGrid
等商业/独立扩展、系统原生菜单/托盘/文件选择器、布局与渲染基类不在这批可领取的视觉控件卡中；后续按真实需求拆卡，不能据此宣称已覆盖所有
Avalonia 公共类型。

一张卡先做端点与交互契约，再做稳定视觉，再迭代高级模式。高级功能发现超出单卡范围时，集成人创建后续卡、分配不重叠的里程碑/所有权，再更新本表映射；不要让领取者随手扩张到相邻组件。用户对视觉的反馈回到原任务，公共资源反馈交
F01，最终实现与用户确认范围一起记录。

原生首轮已完成构建和聚焦验证，状态为视觉待验收；N11 Spinner、N26 编辑/排序等后续里程碑仍按实际能力另行推进。E04/E15 独立依赖与 C 系列公开用法仍需对应决策，不因原生主题已接入就认定实现或验收完成。

