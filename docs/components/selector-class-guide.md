# 选择器与 Classes 编写指南（人类）

这份指南用于日常查语法、组织状态样式和排查覆盖问题。团队约束以 [AI 执行规范](selector-class-rules.md) 的规则编号为准；这里只解释如何使用。当前示例按 Avalonia 12.1.3 核对，项目版本仍以中央包文件为准。

## 先选对工具

通常先问：“这是值、外观变体，还是控件自身的状态？”

| 要做的事 | 写法/入口 | 原因 |
| --- | --- | --- |
| 禁用按钮 | `IsEnabled="False"` | 真正阻止交互，同时驱动 `:disabled`。单加一个灰色 class 不会禁用行为。 |
| 切换选中值 | `IsChecked="{Binding ...}"` | 行为和视觉使用同一个值，样式读 `:checked`。 |
| 切换已实现的外观变体 | `Classes="已登记的名称"` | 保持原生控件类型，同一个模板按用途呈现。 |
| 使用窗口关闭按钮外观 | `Theme="{DynamicResource Vhilz.CaptionButton.Theme}"` 配合 `VhilzCaptionClose` | 这是现有专用 Theme 中的角色，不是普通 Button 的通用危险按钮变体。 |
| 换整个模板 | 命名 ControlTheme / BasedOn | 一个 Theme 可以管理自己的模板与状态。 |
| 改某个实例的宽度、对齐 | 属性或父布局 | 不需要建立一套尺寸工具类。 |

Classes 名称与 C# 的 class 类型声明无关。`Vhilz.Brush.Text` 是资源键，`VhilzCaptionClose` 是 class；不要把资源键的点分规则搬到 class 名上。[S2/C1]

## 选择器语法速查

下表的 `.a`、`.b`、`.large` 只是解释语法的占位名，**不是本库提供的 Classes**。真实代码遵守 C1 的 `Vhilz<家族><用途>` 命名。示例是选择器片段，需要放在正确的 Style/ControlTheme 上下文；不是全部都适合放入全局样式。

| 选择器 | 匹配含义 | 编写提示 |
| --- | --- | --- |
| `Button` | StyleKey 为 Button 的控件 | 不自动包含所有 CLR 派生类。 |
| `vhz\|TitleBar` | 指定 XAML 命名空间的控件类型 | AXAML 元素写 `vhz:TitleBar`，Selector 中写 `vhz\|TitleBar`；竖线不带反斜杠，表格转义仅用于 Markdown。 |
| `:is(Button)` | 按 StyleKey 类型关系包含派生类型 | 只有确实需要扩大范围才用。 |
| `Button#SaveButton` | 名称为 SaveButton 的 Button | `#` 指 Name/x:Name，不是资源 x:Key。 |
| `Button.a` | 含 a 的 Button | Classes 不是 CSS 字符串。 |
| `Button.a.b` | 同时含 a 和 b 的 Button | AND；不是在两个类中任选一个。 |
| `Button.a, Button.b` | 含 a 或 b 的 Button | OR；每个逗号分支都是完整选择器。 |
| `Button:pointerover` | 指针悬停 | Avalonia 使用 pointerover，不是 CSS hover。 |
| `Button:pressed` | Button 处于按下状态 | 不要套到不提供 pressed 的普通控件上。 |
| `CheckBox:checked` | CheckBox 被选中 | 不确定状态等其他伪类按该控件源码核对。 |
| `Button:focus-visible` | 应显示焦点提示 | 与焦点本身 `:focus` 不完全等价。 |
| `Border:focus-within` | 自身或后代有焦点 | 适合容器；最终命中对象仍是 Border。 |
| `Button:disabled` | 控件实际不可用 | 包含继承禁用状态的情况，别只观察本地 IsEnabled。 |
| `Button[IsDefault=True]` | 属性等于指定值 | 不是 Binding 表达式；值按属性类型转换。 |
| `TextBlock[(Grid.Row)=0]` | 附加属性匹配 | 附加属性名加括号。自定义命名空间类型使用竖线。 |
| `Button:not(:disabled)` | 排除禁用按钮 | 多个排除条件可串联。 |
| `StackPanel > Button` | StackPanel 的直接逻辑子控件 | 项目 AXAML 属性内写成 `StackPanel &gt; Button`。 |
| `StackPanel Button` | StackPanel 的任意层级逻辑后代 Button | 会比 `>` 命中更广；可能误伤内容区。 |
| `Button /template/ ContentPresenter#PART_ContentPresenter` | Button 模板内指定呈现器 | `/template/` 用于模板边界，不是普通后代空格。 |
| `^:pointerover` | 嵌套样式所锚定控件的悬停状态 | `^` 只能在相应嵌套上下文使用。 |
| `^ /template/ Border#PART_FocusRing` | 主题宿主模板中的焦点层 | 在本库 Button 模板中有这个部件，不表示其他模板都有。 |
| `^ TextBlock` | 从父选择器继续选逻辑后代 | 若外层已选到部件，`^` 指那段父选择器，不会重置到根控件。 |
| `TextBlock:nth-child(2n+1)` | 同级序号中的第 1、3、5…项，并要求为 TextBlock | 从 1 计数，不是只数 TextBlock 的 nth-of-type。虚拟化列表不要靠视觉容器次序推业务序号。 |
| `TextBlock:nth-last-child(1)` | 从后往前的第 1 项，并要求为 TextBlock | 按实际兄弟集合计算；odd/even 也可用于 nth-child。 |

原生控件的 `:selected`、`:expanded`、`:error`、`:flyout-open` 等必须逐类型核对；不是任意控件通用关键字。不要照抄 Web 的 `:hover`、`:active`、`:has(...)`、`+`、`~`、`!important` 等语法假设它们在此版本可用。本项目默认只用已核对的选择器；其他语法要先核实当前 Avalonia 解析器再讨论使用。[S3/S4]

### 读懂一条真实选择器

```xml
<Style Selector="^:focus-visible:not(:disabled) /template/ Border#PART_FocusRing">
    <Setter Property="IsVisible" Value="True" />
</Style>
```

把它放在本库 Button 的 ControlTheme 内：`^` 锚定使用这个主题的 Button；宿主有可见焦点且未禁用；进入它的模板；找到名为 PART_FocusRing 的 Border；只改变该 Border 的可见性。它不改变其他按钮或内容中的 Border。

## 三种顺序要分开理解

### 1. 同一条选择器怎么排

按 S3：**类型/`^` → 名称 → class → 属性条件 → 伪类 → 排除条件**；选中模板部件时再接 `/template/ 类型#名称`。

```xml
<Style Selector="^.VhilzCaptionClose:pressed:not(:disabled)">
    <Setter Property="Background"
            Value="{DynamicResource Vhilz.CaptionButton.Close.Pressed.BackgroundBrush}" />
</Style>
```

该片段用于现有 CaptionButton Theme。将 class 写在 pressed 前面便于阅读，并不是给它增加权重。`^:pressed` 和 `^ :pressed` 差了一个空格，后者就变成了选后代。

### 2. 文件里的 Style 怎么排

按 S5：**基础 Setter/模板 → 配置 → 变体常态 → 持续状态 → 悬停 → 按下 → 语义限制/提示 → 禁用 → 焦点层**。

下面是对本库 Button 状态组织方式的说明片段，放在已有主主题中理解；它不是另一份可直接覆盖生产文件的完整模板：

```xml
<Style Selector="^[IsDefault=True]">
    <Setter Property="BorderBrush" Value="{DynamicResource Vhilz.Brush.TextSecondary}" />
</Style>
<Style Selector="^:pointerover:not(:disabled):not(:pressed), ^:flyout-open:not(:disabled):not(:pressed)">
    <Setter Property="Background" Value="{DynamicResource Vhilz.Brush.ControlHover}" />
</Style>
<Style Selector="^:pressed:not(:disabled)">
    <Setter Property="Background" Value="{DynamicResource Vhilz.Brush.ControlPressed}" />
</Style>
<Style Selector="^:disabled">
    <Setter Property="Opacity" Value="{DynamicResource Vhilz.Opacity.Disabled}" />
</Style>
<Style Selector="^:focus-visible:not(:disabled) /template/ Border#PART_FocusRing">
    <Setter Property="IsVisible" Value="True" />
</Style>
```

悬停排除按下后，二者不会同时竞争背景；禁用排除了交互染色；焦点独立画在自己的部件上。实际模板允许使用已明确的同级声明顺序处理冲突，但不能依赖“选择器更长所以赢”。

有多个变体时先写所有变体的普通态，再写所有变体的悬停态、按下态。`Outline` 普通态与 `:pointerover` 都可能是 StyleTrigger；如果把它的普通态放到最后，它可能盖住早先写的悬停色。[S5/S6]

### 3. Classes 的值怎么排

按 C2：**角色 → 外观 → 密度 → 内容 → 方向 → 功能标记 → 内部状态**。

下例仅演示命名与顺序，**这三个类当前尚未实现**：

```xml
<Button Classes="VhilzButtonDestructive VhilzButtonOutline VhilzButtonIconOnly" />
```

每个名字都要先有消费者和契约才能交付。它们的排列只统一阅读顺序；不会让最后一个类拥有更高覆盖权。Primary 与 Destructive 若被定义为互斥角色，就不能一起设置。

实际已有的 class 用法：

```xml
<Button Theme="{DynamicResource Vhilz.CaptionButton.Theme}"
        Classes="VhilzCaptionClose"
        Content="关闭预览" />
```

这只是现有关闭角色的外观演示，没有自动附加关闭窗口行为；真实按钮需要对应的命令/事件或库窗口模板。已支持的名称与作用域见 [S7 名称记录](selector-class-rules.md#s7--已有名称与迁移边界)。

## 条件 class 和状态的唯一来源

布尔绑定使用 `Classes.<名字>`；仓库已有内部投射写法如下，仅适用于窗口标题相关主题的模板上下文：

```xml
Classes.VhilzInactive="{Binding !IsActive, RelativeSource={RelativeSource TemplatedParent}}"
```

它来自窗口激活属性，并不授予调用者任意操纵窗口视觉状态的公共 API。新条件类同样只允许一个写入方；如果绑定在维护成员，就不要在 Click 事件中再 Add/Remove 同一成员。[C2/S7]

原生禁用、选中、展开走原生属性；自定义控件确有新状态时，由控件自身维护伪类。不要为了把按钮染灰，造一个“disabled class”但让点击继续执行。

## 为什么我的样式没生效

Avalonia 对同一对象同一 StyledProperty 的常见优先级是：

| 从高到低 | 典型来源 |
| --- | --- |
| Animation | 当前生效的动画值。 |
| LocalValue | 消费者在控件属性上直接设置的值/绑定。 |
| StyleTrigger | class、伪类、属性条件激活的样式值。 |
| Template | 模板内设置的值/绑定。 |
| Style | 普通主题 Setter、类型/名称匹配等无条件样式。 |
| Inherited / 默认值 | 继承值或属性元数据默认值。 |

同优先级再看作用域和声明顺序。同一 Styles 集合中后声明通常优先；更贴近控件的样式作用域也会影响结果。**没有 CSS 的 ID、class、类型计分规则**，两个条件样式不会因一个多写三个 class 就自动更强。

例如应用直接写 `Button.Background="..."`，状态 Setter 再设置同一 Button.Background，局部值优先。若状态 Setter 改的是内部 ContentPresenter.Background，就绕到了另一个对象，仍可能盖掉想要的显示。要保留应用覆盖，优先让状态改 Button.Background，再由模板绑定传递给内部呈现器。[S4/S6]

推荐按这个顺序排查：

1. 控件实际用了哪个 Theme，StyleKey 是什么，目标部件是否真的存在。
2. 选择器是否命中正确的逻辑树/模板树；Popup 内容是否处于预期作用域。
3. class/伪类的大小写和状态是否真实成立，是否存在互斥 class 同时被设置。
4. 冲突是否发生在同一对象同一属性；属性面板显示的最终值来源是什么。
5. 是否有局部值、Template 值、更近作用域或后声明样式；动画结束后结果是否不同。

默认部件属性最好直接写在模板中，需外部覆盖的值使用 TemplateBinding。若普通 `^ /template/ Border#Name` 的 Setter 覆盖不了模板里已设置的属性，不要靠增加 `#Name` 或重复类型来“加权”。

## 几种常见误写

| 误写/做法 | 问题 | 应采用的方式 |
| --- | --- | --- |
| `Classes="A,B"` | 多 class 不是逗号分隔。 | `Classes="A B"`；真实名称要遵守命名规范。 |
| `Button.A B` | 空格选后代，B 被当作类型，不是第二个 class。 | 同一控件用 `Button.A.B`。 |
| `^.A, .B` | 第二分支丢掉宿主，主题上下文也可能不合法。 | `^.A, ^.B`。 |
| 用 `Button:checked` 表示按下 | 普通 Button 的按下是 pressed，checked 属于可切换控件。 | 按实际类型使用 pressed 或选用 ToggleButton。 |
| 全局 `Button TextBlock` | 影响调用方内容中的文字，且未进入按钮模板。 | 默认前景放宿主，通过继承/绑定传递；部件差异在所属模板处理。 |
| 在 Demo 写 `/template/` 修组件 | 外观只在一个页面正确，生产实现仍错误。 | 回到组件任务拥有的主题。 |
| 加 `VhilzDark`/`VhilzLight` | 与 RequestedThemeVariant 形成第二套状态来源。 | 同名语义资源放 Light/Dark 字典。 |
| 给禁用态最后设置所有颜色 | 清除了选中/错误信息，未必符合用户需要。 | 按属性定义冲突表，禁用只处理应弱化和应禁止的部分。 |

## 团队交付时写什么

在自己的 [交付包](delivery-template.md) 写 class 名称、类型范围、公开/内部、所属维度、互斥与可组合项、默认表现、谁负责切换以及 Demo。再写同一属性同时被多个状态影响时的结果和选择器边界。集成人检查共享名称与重复类型键；不用每位作者直接改全局 CommonStyles 或公共记录。

新增/调整主题时按项目既定强度构建与验证，检查明暗、真实交互和显式属性覆盖。只查语法或改文档不需要创建专门测试工程；本指南中的说明片段并未被当作独立应用运行。

## 官方参考

- [选择器语法完整参考](https://docs.avaloniaui.net/docs/styling/style-selector-syntax)
- [Classes 与条件类](https://docs.avaloniaui.net/docs/styling/style-classes)
- [伪类定义与维护](https://docs.avaloniaui.net/docs/styling/pseudoclasses)
- [ControlTheme 查找与 BasedOn](https://docs.avaloniaui.net/docs/styling/control-themes)
- [属性值优先级与声明顺序](https://docs.avaloniaui.net/docs/properties/value-precedence)
