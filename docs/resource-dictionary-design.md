# 资源字典设计

现行资源结构采用语义 Token 与按需控件参数，保持明暗字典键及类型对应；命名以 [资源键规范](resource-key-naming.md) 为准。

## 资源归属

| 位置 | 职责 |
| --- | --- |
| `Themes/VhilzTheme.axaml` | 加载明暗主题字典、公共 Token、控件主题及独立选择器样式 |
| `Themes/Resources/Light.axaml`、`Dark.axaml` | 共享用途色与控件状态画笔，键及类型对应 |
| `Themes/Resources/Tokens.axaml` | 跨主题的稳定尺寸、间距等公共参数 |
| `Themes/Controls/*.axaml` | 控件主题、模板和仅服务该模板的实现参数 |

当前控件数量较少，继续直接加载控件字典；增加控件后按维护负担决定聚合入口。颜色命名继续遵循 [资源键规范](resource-key-naming.md)。

## 共享色与局部画笔

本轮提取六个已有用途，默认色值保持当前工作区配色：

| 共享 Color 键 | 当前默认消费者 |
| --- | --- |
| `Vhilz.Color.Surface` | 窗口背景、透明回退底色、标题栏背景 |
| `Vhilz.Color.Text` | 窗口正文前景 |
| `Vhilz.Color.TextSecondary` | 标题栏文字、窗口按钮常态图标 |
| `Vhilz.Color.TextMuted` | 非激活标题与窗口按钮图标 |
| `Vhilz.Color.Border` | 激活窗口描边 |
| `Vhilz.Color.BorderInactive` | 非激活窗口描边 |

共享色通过 `SolidColorBrush.Color="{DynamicResource ...}"` 驱动现有控件画笔；模板按用途动态引用语义或确需局部覆盖的控件画笔键。这样可同时调整共同视觉用途，也能单独替换窗口或标题栏的画笔。

语义 Brush 对应已有 Color 用途，按钮常态/非激活图标直接复用 TextSecondary/TextMuted；OnDanger 为跨主题不变的白色交互前景。关闭按钮状态背景与辉光仍有自身语义，保留控件键。尚未使用的色板不预建。

## 覆盖范围与资源宿主

动态画笔中的 Color 由**定义该画笔的资源宿主**解析，不按每个消费控件重新查找。常规应用将 `VhilzTheme` 加入 `Application.Styles` 时，在 `Application.Resources` 中覆盖语义 Color；单窗口或单控件差异覆盖原有 `Vhilz.Window.*Brush` / `Vhilz.CaptionButton.*Brush`。

```xml
<!-- 应用级：所有默认表面共同使用新底色。 -->
<Color x:Key="Vhilz.Color.Surface">#202024</Color>

<!-- 窗口级：只改变该窗口标题栏，正文与其他窗口仍使用共享色。 -->
<SolidColorBrush x:Key="Vhilz.Window.TitleBar.BackgroundBrush">#282830</SolidColorBrush>
```

上例分别放在对应宿主的 Resources 中。需要不同明暗值时放入该宿主的 `ThemeDictionaries`，使用相同键。替换或移除资源应立即更新已有控件；显式设置控件属性仍优先于主题 Setter。

独立构造、尚未挂载宿主的 `VhilzTheme` 可以检查资源键与类型，但其中依赖动态颜色的画笔尚未解析，不能据此断言最终颜色。运行时测试应从挂载后的窗口查找资源。

## 标题栏与验收

TitleBar 从所属 `Window.Title` 实时读取文字。自定义 LeftContent 仍位于左侧，标题使用其后的可用区域，CenterContent 与 RightContent 保留原内容对象；长标题省略、空标题隐藏。标题文字不接收命中，保留上层标题栏拖动语义，并继承激活/非激活前景。

窗口与独立 TitleBar 提供 `IsTitleVisible`（默认 `True`）及 `TitleAlignment`（`Left` / `Center`，默认 `Left`）。居中以标题可用区域为基准，避让内容槽和窗口按钮；隐藏只影响标题文字，不清空 `Window.Title`，也不隐藏整个标题栏。普通装饰与全屏浮层标题同步响应窗口设置。

```xml
<vhz:VhilzWindow Title="示例窗口" IsTitleVisible="True" TitleAlignment="Center" />
```

Demo 中可切换标题显示与对齐方式，标题输入框支持普通、长文本及空标题检查。运行：

```powershell
dotnet build Vhilz.Avalonia.sln --artifacts-path ./artifacts/title-settings
dotnet test Vhilz.Avalonia.Tests/Vhilz.Avalonia.Tests.csproj --artifacts-path ./artifacts/title-settings
dotnet ./artifacts/title-settings/bin/Vhilz.Avalonia.Demo/debug/Vhilz.Avalonia.Demo.dll
```

切换明暗主题和窗口激活状态；编辑标题、缩窄窗口；拖动标题区域、双击最大化/还原，检查窗口按钮与全屏浮层。自动测试只覆盖字典键/类型、颜色链路、动态覆盖和标题结构行为。材质、排版观感、系统拖动与不同缩放比例待用户实际运行验收；Linux、macOS 尚未验证。

## 非画笔值

模板直接动态引用语义圆角、时长和透明度。StaticResource 别名不能保留运行时原键覆盖关系，见 ResourceDictionaryTests 的加载时别名与应用级覆盖回归。单个控件差异通过公开属性或样式 Setter 实现，避免为数值复制控件键。
