# ShadUI 资源字典设计对照

> 历史参考：记录当时的调研或诊断，可能包含已替换实现；现行规则与待办以 [资源键命名](resource-key-naming.md) 和 [优化任务清单](optimization-tasks.md) 为准。
研究日期：2026-10-04。上游固定在 [`ff1e84698d1478fc6403175858a2fd5303ad28df`](https://github.com/accntech/shad-ui/tree/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Themes)，本仓库按当前工作区读取。本文是设计建议，不表示已采用新的资源契约；仅新增文档，未修改主题代码。

## 结论

最值得借鉴的是：让多个控件共享一组按用途命名的颜色，并把背景与其配套前景作为一组设计。Vhilz 已有明暗字典、公共 Token、控件模板和动态资源引用，基础组织方式无需推倒重做；下一步的价值在于从现有窗口、标题栏专用资源中，逐步提炼真正可供 Button、TextBox、弹层等复用的语义资源。

## 已有相同基础，不必重新设计

ShadUI 的入口将资源和选择器样式分开加载：[ShadTheme.axaml 第 7–17 行](https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Themes/ShadTheme.axaml#L7-L17) 用 `MergedDictionaries` 加载常量及控件资源，用 `ThemeDictionaries` 加载 `Light`/`Dark`，最后用 `StyleInclude` 加载选择器样式。

Vhilz 的 [VhilzTheme.axaml](../Vhilz.Avalonia/Themes/VhilzTheme.axaml) 已采用同一基础结构：`Tokens.axaml`、三个控件主题、明暗字典、`CommonStyles.axaml` 分别承担对应职责。继续使用 Avalonia 原生主题变体即可，无需为借鉴 ShadUI 再造主题切换机制。

上游 Light/Dark 均有 58 个键，键集合一致；这是本次对两个 XML 文件的静态检查结果。Vhilz 已要求明暗键对应，也有运行时主题切换与覆盖测试，例如 [WindowPaletteTests.cs](../Vhilz.Avalonia.Tests/WindowPaletteTests.cs)、[DemoThemeTests.cs](../Vhilz.Avalonia.Tests/DemoThemeTests.cs)。后续扩展公共语义资源时可增加字典键和资源类型一致性检查，防止漏补其中一个主题。

## 最值得吸收的设计

| 上游设计与证据 | 对 Vhilz 的意义 |
| --- | --- |
| 基础颜色使用 `ForegroundColor`、`MutedColor`、`BorderColor` 等用途名，而非要求控件认识某个灰阶编号。[Dark.axaml 第 3–15 行](https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Themes/Dark.axaml#L3-L15) | 在新增普通控件时建立少量共享语义，例如 `Vhilz.Global.Text.Primary.ForegroundBrush`、`Vhilz.Global.Text.Secondary.ForegroundBrush`、`Vhilz.Global.Surface.BorderBrush`。这些仅为候选键，沿用现有 `Global` 作用域和用途结尾规则。 |
| `PrimaryColor`/`PrimaryForegroundColor`、`SecondaryColor`/`SecondaryForegroundColor`、`DestructiveColor`/`DestructiveForegroundColor` 成对出现。[Dark.axaml 第 17–31 行](https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Themes/Dark.axaml#L17-L31)、[Light.axaml 第 17–31 行](https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Themes/Light.axaml#L17-L31) | 主操作色、危险操作色应与其表面上的文字/图标色一起设计和覆盖，避免只改背景而损失可读性。这里的 `Primary` 是主要操作角色；ShadUI 默认是随明暗反转的中性色，不应直接理解为用户可配置的品牌强调色。 |
| Button 和 Badge 共同消费 `Primary`、`Secondary`、`Destructive` 颜色对。[Button.axaml 第 166–216 行](https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Button/Button.axaml#L166-L216)、[Badge.axaml 第 124–166 行](https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Badge/Badge.axaml#L124-L166) | 真正的收益是跨控件共用视觉语言。共享颜色不等于共享全部状态；窗口关闭按钮仍可保留自身 `PointerOver`/`Pressed` 资源，不必机械合并进普通危险按钮。 |
| `Info`、`Success`、`Warning`、`Error` 在明暗主题各自定值。[Dark.axaml 第 33–53 行](https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Themes/Dark.axaml#L33-L53)、[Light.axaml 第 33–53 行](https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Themes/Light.axaml#L33-L53) | 新增校验、通知等控件时，应先定义成功/警告/错误角色，再给具体控件映射。明暗方案分别调色，不能假设一组 RGB 在两个背景上都有足够对比度。源代码分开定色不等于已证明对比度合格。 |
| 通用颜色之外仍保留 `CardBackgroundColor`、`DialogBackgroundColor`、`WindowBackgroundColor` 等控件专用入口。[Dark.axaml 第 55–73 行](https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Themes/Dark.axaml#L55-L73) | 保留 Vhilz 当前窗口、标题栏和 CaptionButton 的局部可覆盖性。建立公共语义层不意味着删除控件专有资源；玻璃容器所需的叠色、回退底色、描边等也不能仅由一个全局背景色表达。 |

以上是语义共享的事实和可迁移思路。ShadUI 的 Light/Dark 本身混合了基础用途色、操作角色色和控件专用色，**并不是完整的“原始色板 → 语义资源 → 控件资源”三层体系**。如果 Vhilz 采用这样的分层，应视为我们自己的增量设计。

## 颜色类型与资源粒度

ShadUI 的两个主题字典只定义 `Color`，控件直接把 `DynamicResource` 指向这些颜色；例如 [Button.axaml 第 45–55 行](https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Button/Button.axaml#L45-L55) 将其用于 `Background`、`Foreground`。它没有在 Themes 中给每个颜色机械地再包一层 Brush。

Vhilz 当前的 [Light.axaml](../Vhilz.Avalonia/Themes/Resources/Light.axaml) 与 [Dark.axaml](../Vhilz.Avalonia/Themes/Resources/Dark.axaml) 以 `SolidColorBrush` 为主，揭示描边则使用 `Color`，对应消费属性的类型。建议保留这种按用途选类型的方式：只有同一种语义确实同时用于 Brush 属性与材质的 Color 属性，才考虑增加 `Color` 底层和动态关联的 Brush。不要为了形式完整把所有键一律拆成两份；新增间接层后还需验证主题切换、局部覆盖、资源替换是否沿链路生效。

## 规模增长后可借鉴的目录组织

ShadUI 使用 [Controls/Resources.axaml](https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Resources.axaml) 聚合每个控件目录的 ResourceDictionary；[Controls/Styles.axaml](https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Styles.axaml) 聚合选择器样式。这样主题入口不随控件数量膨胀。

Vhilz 目前仅在入口加载三个控件主题，尚无需新增聚合文件。等覆盖更多 Avalonia 默认控件和自定义控件后，可增加类似聚合入口；是否进一步分组，应以实际维护负担决定。ShadUI 的 `Styles.axaml` 还加载了 `SimpleTheme`，这是它的基础主题选择，不应连同组织结构一起照搬到 Vhilz。

## 不建议照搬的部分

- **百分比后缀代替用途名。** 上游的 `PrimaryColor50` 等编码透明度；`BorderColor60` 在明暗字典中却都是不透明颜色，说明名称也不总能准确解释值。[Dark.axaml 第 8–10、18–21 行](https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Themes/Dark.axaml#L8-L21)。Vhilz 应继续使用 `PointerOver.BackgroundBrush`、`Disabled.Opacity` 等已有语义规则。半透明表面的最终颜色还依赖底层内容，尤其不能凭透明度编号判断玻璃效果。
- **无命名空间的通用键。** `ForegroundColor`、`BorderColor` 等很简短，但 Vhilz 要同时集成其他主题库，继续使用 `Vhilz.` 前缀更利于定位与避免碰撞。无需回退现有点分命名；规范见 [resource-key-naming.md](resource-key-naming.md)。
- **提前复制一套全局尺寸档位。** 上游 [Constants.axaml 第 3–11 行](https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Controls/Constants.axaml#L3-L11) 提供圆角尺寸序列和单一阴影；它们可以启发共享尺度，但数值不是 Vhilz 已确认的规范。Vhilz 的 [Tokens.axaml](../Vhilz.Avalonia/Themes/Resources/Tokens.axaml) 已对 CaptionButton 的尺寸和动画做了更细的语义化，不应退回散落的硬编码时长；上游 Button 内仍存在固定动画时长。
- **为 AXAML 主题同步增加手工 C# 镜像。** [ThemeWatcher.cs 第 56–124 行](https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Themes/ThemeWatcher.cs#L56-L124) 手工读取一长串键，配合 [ThemeColors.cs](https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Themes/ThemeColors.cs) 暴露颜色快照；这适合有明确非 AXAML 消费者时评估。普通模板继续 `DynamicResource` 即可，否则会多一份必须同步维护的清单。它的 `SwitchTheme` 最终仍是设置 `Application.RequestedThemeVariant`，见 [ThemeWatcher.cs 第 151–160 行](https://github.com/accntech/shad-ui/blob/ff1e84698d1478fc6403175858a2fd5303ad28df/src/ShadUI/Themes/ThemeWatcher.cs#L151-L160)。

## 建议推进顺序

1. 在实现下一批 Button、TextBox 或弹层时，先识别确实共同使用的正文、次级文字、表面、描边及操作颜色对；同步提供明暗定义。
2. 保留已有控件资源契约，用公共语义表达默认设计；只有验证局部覆盖仍然有效后，才引入资源之间的间接关联。不要因为两个当前色值相同就合并资源。
3. 以 Demo 检查明暗、焦点、禁用、悬停与按下，自动测试资源类型、切换和覆盖。玻璃材质与实际视觉可读性仍由用户运行验收。
4. 控件数量增加后再抽取聚合文件和必要的公共尺寸尺度，不在当前三个控件主题的规模提前扩张目录。

本次完成的是源码和资源结构检查，未运行上游或本项目构建、测试与视觉验收；未验证其他平台表现。
