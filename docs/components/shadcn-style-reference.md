# shadcn 原版样式参考与原生映射

本轮参考官方仓库 `shadcn-ui/ui` 的固定快照 `6ea090075cd537d3b792c6c1a625e2448b6ede26`，主要读取
`apps/v4/registry/new-york-v4/ui`；`styles/style-vega.css` 与 Base/Radix 实现用于补充核对结构。
参考源码位于本机临时目录 `C:\Users\SkyeJ\AppData\Local\Temp\vhilz-style-reference\shadcn`，不进入产品或构建依赖。
已通过 shadcn CLI 获取 Button、Input、Checkbox、RadioGroup、Switch、Slider、Progress、ScrollArea、Select、Tabs、Tooltip、DropdownMenu、Calendar、Toast 的官方文档入口，并下载核对。

原生行为模板参考 [Avalonia 12.1.3](https://github.com/AvaloniaUI/Avalonia/tree/12.1.3/src/Avalonia.Themes.Simple/Controls)，
固定提交 `8eeda4f6f546165b3f72e63c9f42247abb306905`。复用其模板组成及必要绑定，再用 Vhilz 语义资源定义视觉。
库不依赖 Simple/Fluent 主题包，不添加运行时回退。

| 原版样式证据 | 本轮映射与取舍 |
| --- | --- |
| Button 使用 `text-sm`、`rounded-md`、Primary/Secondary/Outline/Ghost 等独立角色 | 保留现有普通按钮默认中性表面，不自行新增变体契约；复用 14 DIP 正文字号、5 DIP 圆角、32 DIP 既有高度 |
| Input/Select 使用细边框、弱背景、muted 占位，校验使用 destructive | 输入使用 Input/Border/TextMuted，校验使用 Danger；保留原生文本编辑及校验机制 |
| Checkbox/Radio/Switch 使用 primary 表达选中，轮廓或轨道保持中性 | Primary 在明色为近黑、暗色为近白，OnPrimary 为相反前景；勾选与不确定态由原生状态产生 |
| Slider/Progress 为窄轨道与圆形端点，未完成部分为 muted | 轨道与填充分别消费 Control/Primary；不增加自有时间轴 |
| Tabs 为 muted 底座、background 活动项，焦点单独呈现 | 保持原生 TabControl/TabStrip 内容选择与布局契约；选中不冒充键盘焦点 |
| Select/Menu/Popover 使用 popover 表面、细边框、轻阴影及 accent 高亮 | Popup/Border/Selection/OnSelection 和 Shadow.Popup；弹层继续使用原生 Popup 与展示器 |
| Tooltip 使用逆色背景与小字号 | 使用 Primary/OnPrimary 和 Caption 字号；保留原生定位和打开机制 |
| 表格以行分隔线、行悬停/选中建立层次 | 基于 Avalonia TableView 原生行、列头、单元格和滚动；不宣称分页、筛选或 DataGrid 能力 |
| 原版普通控件几乎没有材质效果 | 普通控件使用简单表面；本轮不将玻璃扩散到所有控件 |

本轮参数是可覆盖、待视觉验收的候选，不确立全局 Small/Medium/Large 密度档位。
资源直接使用 DynamicResource；应用级 Color 覆盖和单控件公开属性覆盖保持现有规则。
新增静态状态外观不添加动画曲线。既有 Button/窗口动效和上游必要运行机制保留，由用户继续打磨。

本轮原生范围为 F00/F01、N01–N28，以及无需新增包的 E01–E03、E05–E14。
E04 ColorPicker、E15 DataGrid 需要独立包决策，C 系列组合/自研组件不属于这次原生外观交付。
进度与实际里程碑见 [领取记录](claims.md)，视觉通过只能由用户确认。
