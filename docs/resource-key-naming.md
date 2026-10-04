# 资源键命名规范

状态：已采用，项目自有的 37 个字符串资源键已迁移为点分层级。旧键不保留别名，应用端的资源覆盖和字符串查找需按下表同步更新。

## 1. 基本形式

采用 **点分层级、段内 PascalCase、语义属性结尾**：

```text
Vhilz.<作用域>[.<变体>][.<部件>].<用途路径>
```

点用于分隔语义层级，复合术语保留为一段：`CaptionButton`、`PointerOver`、`CornerRadius`、`StrokeWidth`。不逐词拆成 `Caption.Button`，不使用缩写 `Bg`、`Fg`、`Dur`。

例如：

```text
Vhilz.CaptionButton.Width
Vhilz.CaptionButton.Close.PointerOver.BackgroundBrush
Vhilz.CaptionButton.Motion.Color.Default.Duration
```

依次回答“属于谁、哪种变体或哪个部件、什么用途”。长度不作为首要目标；优先让名称可分段扫描、可按前缀搜索、能区分相近资源。

## 2. 作用域、变体与部件

- **作用域**使用稳定的视觉对象：`Window`、`TitleBar`、`CaptionButton`、`Button`、`TextBox`。已有对象的含义见 [主题词汇表](../CONTEXT.md)。不在作用域重复 `Vhilz`，例如使用 `Vhilz.Window.Theme`。
- **变体**紧跟作用域：`CaptionButton.Close`、`Button.Primary`。变体不等于交互状态；避免 `CloseButton`、`CaptionCloseButton` 等多套叫法。
- **部件**只在需要定位时出现：`Window.TitleBar`、`CaptionButton.Icon`、`CaptionButton.Icon.Restore`、`CaptionButton.RevealBorder`。独立控件主题用 `TitleBar.Theme`，窗口中标题栏的布局参数用 `Window.TitleBar.Height`。
- **全局共享语义**以 `Global` 为作用域，例如 `Vhilz.Global.Motion.Fast.Duration`。只有确实建立共同设计语义时才提升为全局资源，数值碰巧相同不代表应共享。
- **设计时资源**以 `Preview` 为作用域，例如 `Vhilz.Preview.WindowDecorations.BackgroundBrush`，避免与应用可覆盖的正式主题资源混淆。

只写实际存在的层级，不加空占位，也不把全部资源统一塞进 `Controls`、`Resources`、`Tokens` 等无助于定位的层级。

## 3. 静态值与状态值

```text
Vhilz.<对象路径>.<属性>
Vhilz.<对象路径>.<状态>.<属性>
```

| 用途 | 名称示例 |
| --- | --- |
| 按钮宽度 | `Vhilz.CaptionButton.Width` |
| 按钮圆角 | `Vhilz.CaptionButton.CornerRadius` |
| 图标描边宽度 | `Vhilz.CaptionButton.Icon.StrokeWidth` |
| 还原图标描边宽度 | `Vhilz.CaptionButton.Icon.Restore.StrokeWidth` |
| 悬停背景画笔 | `Vhilz.CaptionButton.PointerOver.BackgroundBrush` |
| 关闭变体按下背景画笔 | `Vhilz.CaptionButton.Close.Pressed.BackgroundBrush` |
| 禁用透明度 | `Vhilz.CaptionButton.Disabled.Opacity` |
| 按下时表面变换目标值 | `Vhilz.CaptionButton.Surface.Pressed.RenderTransform` |

状态沿用 Avalonia 语义：`Normal`、`PointerOver`、`Pressed`、`Disabled`、`Focused`、`Selected`、`Checked`、`Indeterminate`；不混用 `Hover`/`PointerOver`、`Press`/`Pressed`、`Disable`/`Disabled`。

无状态段表示通用配置；明确的常态外观使用 `Normal`。若确实存在组合状态，固定顺序为“选择状态 → 指针/按压状态 → 焦点状态”，例如 `Checked.PointerOver.BackgroundBrush`；禁用资源单独命名。组合键的存在不会自动建立样式优先级，仍需模板明确处理。

属性名尽量匹配所表达的 Avalonia 属性，必要时明确资源类型：

- `Color` 表示 `Color` 值；`BackgroundBrush`、`ForegroundBrush`、`BorderBrush` 表示画笔。不能将画笔资源命名为 `BackgroundColor`。
- `CornerRadius` 表示几何圆角；`RevealBorder.Radius` 表示揭示效果影响半径，二者不能都简写为 `Radius`。
- `Duration`、`Easing`、`Opacity`、`Thickness`、`Geometry`、`RenderTransform` 已表达用途，不再追加 `TimeSpan`、`Double`、`Value`、`Token`。
- 不在键中编码具体值、单位、色号、缓动实现类或主题变体。例如使用 `CornerRadius`，不使用 `Radius3`；使用 `Easing`，不使用 `SineEaseInOut`。单位及特殊坐标系留在必要说明中。

## 4. 动画参数必须说明目标和方向

```text
Vhilz.<对象路径>.Motion.<动画目标>.<过渡阶段>.<参数>
```

| 层级 | 含义与词汇 |
| --- | --- |
| 动画目标 | `Background`、`Foreground`、`RenderTransform`、`Opacity`；背景与前景明确共用的颜色过渡参数使用 `Color` |
| 过渡阶段 | `Default` 表示该目标的通用过渡参数；`ToPressed`、`ToNormal`、`ToPointerOver` 表示进入目标状态；必要时用 `FromPressedToNormal` 表达限定起点与终点的过渡 |
| 参数 | `Duration`、`Easing`、`Delay` 等 |

阶段不使用含义模糊的 `Press`、`Exit`。按下与释放共用的变换时长使用 `RenderTransform.Default.Duration`；只在进入按下态使用的背景时长使用 `Background.ToPressed.Duration`。状态目标值仍按上一节命名，不放进 `Motion`。

原 `VhilzCaptionButtonColorDuration` 同时被背景染色和关闭按钮前景过渡引用，因此命名为：

```text
Vhilz.CaptionButton.Motion.Color.Default.Duration
```

若后续需要独立调节背景与前景，应明确拆为 `Motion.Background.Default.Duration` 和 `Motion.Foreground.Default.Duration`；这属于资源契约调整，不能通过一次机械重命名悄悄完成。

## 5. 非 Token 资源

同样使用语义路径，以资源角色结尾：

| 资源 | 目标名称 |
| --- | --- |
| 窗口主题 | `Vhilz.Window.Theme` |
| 窗口装饰主题 | `Vhilz.WindowDecorations.Theme` |
| 标题栏主题 | `Vhilz.TitleBar.Theme` |
| 窗口操作按钮主题 | `Vhilz.CaptionButton.Theme` |
| 还原图标路径 | `Vhilz.CaptionButton.Icon.Restore.Geometry` |
| 内部过渡参数选择器 | `Vhilz.CaptionButton.Motion.ParameterSelector.Converter` |
| 设计时行模板 | `Vhilz.Preview.WindowDecorations.Row.DataTemplate` |

命名不决定可见性：内部转换器无需因为拥有资源键就成为公开扩展点。

本规范仅适用于项目自有的**字符串资源键**。上游约定键、`{x:Type ...}` 类型键、`Light`/`Dark` 主题字典键，以及 `PART_*` 部件名、样式类、C# 标识符均保留各自约定。

## 6. 旧键迁移对照

以下映射按引用处的实际用途设计，迁移保留原有默认值及交互逻辑。旧键仅在本表及迁移说明中保留，供应用端更新引用。

| 现有键 | 目标键 |
| --- | --- |
| `VhilzWindowTitleBarHeight` | `Vhilz.Window.TitleBar.Height` |
| `VhilzWindowFrameThickness` | `Vhilz.Window.FrameThickness` |
| `VhilzCaptionButtonWidth` | `Vhilz.CaptionButton.Width` |
| `VhilzCaptionButtonRadius` | `Vhilz.CaptionButton.CornerRadius` |
| `VhilzCaptionButtonMargin` | `Vhilz.CaptionButton.Margin` |
| `VhilzCaptionIconSize` | `Vhilz.CaptionButton.Icon.Size` |
| `VhilzCaptionIconStrokeWidth` | `Vhilz.CaptionButton.Icon.StrokeWidth` |
| `VhilzCaptionRestoreIconStrokeWidth` | `Vhilz.CaptionButton.Icon.Restore.StrokeWidth` |
| `VhilzCaptionButtonPointerOverBackground` | `Vhilz.CaptionButton.PointerOver.BackgroundBrush` |
| `VhilzCaptionButtonPressedBackground` | `Vhilz.CaptionButton.Pressed.BackgroundBrush` |
| `VhilzCaptionCloseButtonPointerOverBackground` | `Vhilz.CaptionButton.Close.PointerOver.BackgroundBrush` |
| `VhilzCaptionCloseButtonPressedBackground` | `Vhilz.CaptionButton.Close.Pressed.BackgroundBrush` |
| `VhilzCaptionCloseButtonPointerOverForeground` | `Vhilz.CaptionButton.Close.InteractionForegroundBrush` |
| `VhilzCaptionButtonDisabledOpacity` | `Vhilz.CaptionButton.Disabled.Opacity` |
| `VhilzCaptionButtonRevealBorderEnabled` | `Vhilz.CaptionButton.RevealBorder.IsEnabled` |
| `VhilzCaptionButtonRevealBorderColor` | `Vhilz.CaptionButton.RevealBorder.Color` |
| `VhilzCaptionButtonRevealBorderWidth` | `Vhilz.CaptionButton.RevealBorder.Width` |
| `VhilzCaptionButtonRevealBorderRadius` | `Vhilz.CaptionButton.RevealBorder.Radius` |
| `VhilzCaptionButtonRevealBorderIntensity` | `Vhilz.CaptionButton.RevealBorder.Intensity` |
| `VhilzCaptionButtonRevealProximityDistance` | `Vhilz.CaptionButton.RevealBorder.ProximityDistance` |
| `VhilzCaptionButtonPressedTransform` | `Vhilz.CaptionButton.Surface.Pressed.RenderTransform` |
| `VhilzCaptionButtonPressDuration` | `Vhilz.CaptionButton.Surface.Motion.RenderTransform.Default.Duration` |
| `VhilzCaptionButtonPressEasing` | `Vhilz.CaptionButton.Surface.Motion.RenderTransform.Default.Easing` |
| `VhilzCaptionButtonColorDuration` | `Vhilz.CaptionButton.Motion.Color.Default.Duration` |
| `VhilzCaptionButtonPressedColorDuration` | `Vhilz.CaptionButton.Motion.Background.ToPressed.Duration` |
| `VhilzCaptionButtonColorEasing` | `Vhilz.CaptionButton.Motion.Color.Default.Easing` |
| `VhilzCaptionCloseButtonExitDuration` | `Vhilz.CaptionButton.Close.Motion.Background.ToNormal.Duration` |
| `VhilzCaptionCloseButtonExitEasing` | `Vhilz.CaptionButton.Close.Motion.Background.ToNormal.Easing` |
| `VhilzWindowTheme` | `Vhilz.Window.Theme` |
| `VhilzWindowDecorationsTheme` | `Vhilz.WindowDecorations.Theme` |
| `VhilzTitleBarTheme` | `Vhilz.TitleBar.Theme` |
| `VhilzCaptionButtonTheme` | `Vhilz.CaptionButton.Theme` |
| `VhilzWindowRestoreIconGeometry` | `Vhilz.CaptionButton.Icon.Restore.Geometry` |
| `VhilzCaptionTransitionConverter` | `Vhilz.CaptionButton.Motion.ParameterSelector.Converter` |
| `VhilzDecorationsPreviewBackground` | `Vhilz.Preview.WindowDecorations.BackgroundBrush` |
| `VhilzDecorationsPreviewForeground` | `Vhilz.Preview.WindowDecorations.ForegroundBrush` |
| `VhilzDecorationsPreviewRowTemplate` | `Vhilz.Preview.WindowDecorations.Row.DataTemplate` |

`InteractionForegroundBrush` 明确表示当前悬停和按下共用的前景画笔，避免旧名称只写 `PointerOver` 却也用于 `Pressed`。它不自动涵盖禁用或焦点状态。若将来两种状态需分别覆盖，应另行设计两个状态键。

`ToNormal` 对应当前关闭按钮背景回到透明的分支；它不表示控件退场，也不表示窗口关闭动画。名称表达目标视觉状态，不假设只能由指针移出触发。

## 7. 定义、引用与注释

- 随明暗变化的资源在 `Themes/Resources/Light.axaml` 和 `Dark.axaml` 使用同一键；跨主题不变的资源放在 `Themes/Resources/Tokens.axaml`。键中不加入 `Light`/`Dark`。
- 需要主题切换或应用覆盖的值继续使用 `DynamicResource`；分隔符不会赋予资源继承、回退或动态更新能力，所有键仍是完整字符串。
- 定义处和引用处都使用完整键名，包括 C# 字符串查找。避免按旧前缀拼接键名而遗漏变体、状态或点号。
- 注释保留单位、坐标换算、平台限制、上游兼容原因和设计取舍；不再逐项注释“按钮宽度”“按钮圆角”等名称已经表达的事实。

写法示例：

```xml
<sys:TimeSpan x:Key="Vhilz.CaptionButton.Motion.Color.Default.Duration">0:0:0.12</sys:TimeSpan>
<BrushTransition Property="Foreground"
                 Duration="{DynamicResource Vhilz.CaptionButton.Motion.Color.Default.Duration}" />
```

## 8. 迁移边界与检查

资源键是应用自定义主题的契约。本次直接切换为新键，不提供旧键兼容层。只覆盖旧键的应用需同步迁移，否则覆盖将不再作用于控件；仅复制一份默认值不能保证旧键覆盖或动态更新继续有效。

已同步定义、模板引用、Demo、测试查找及文档示例。测试使用完整键名，已删除旧前缀拼接。Token 中移除了仅复述名称的注释，保留设计原因、上游默认值和坐标说明。

明色字典补齐 `Vhilz.CaptionButton.RevealBorder.Color`，显式采用原先回退到的 FAA 默认白色；测试核对了上游默认值、明暗资源查找与表面绑定。明暗字典现有资源键一致。

### 自动化验证

在仓库根目录运行：

```powershell
dotnet build Vhilz.Avalonia.sln --artifacts-path ./artifacts/resource-key-migration
dotnet test Vhilz.Avalonia.Tests/Vhilz.Avalonia.Tests.csproj --artifacts-path ./artifacts/resource-key-migration
```

Windows 下独立输出目录构建通过（0 警告、0 错误），15 项测试通过。默认 Demo 输出目录被现有进程锁定，因此使用独立输出目录验证。AXAML 中的点分键已通过编译与运行时解析；测试覆盖明暗切换、背景画笔覆盖、图标尺寸/描边/路径替换，以及背景颜色动画默认、按下和恢复常态分支的运行时时长覆盖。

迁移前有两项测试仍期望 240ms，但现有颜色时长 Token 已为 120ms。本次保留 120ms，修正测试预期及 Demo 提示。

额外探查发现：关闭按钮前景 `BrushTransition.Duration` 在 Headless 下读取为 0，运行时覆盖通用颜色时长也未更新该前景过渡。在迁移前资源及测试的独立副本中可复现，属于已有问题。本轮未修改这一行为；背景动画动态覆盖的测试通过不代表前景动画时长覆盖也已通过。

### 视觉验收入口

```powershell
dotnet ./artifacts/resource-key-migration/bin/Vhilz.Avalonia.Demo/debug/Vhilz.Avalonia.Demo.dll
```

切换浅色与深色，检查标题栏按钮及 Demo 预览的悬停、按下、禁用、Tab 焦点状态，以及最大化后的还原图标。重点观察背景渐变、按下回弹、关闭按钮恢复常态和揭示描边。材质、动画观感及最终视觉效果仍待用户验收；Linux、macOS 未验证。
