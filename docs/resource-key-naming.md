# 资源键命名

状态：语义层方案与本轮时长档位已获用户确认。旧规则“长度不作为首要目标”、Global 作用域及控件优先的长用途链已废弃。

## 现行规则

公共 Token 使用 `Vhilz.<类型>.<用途>`，段内 PascalCase，短而通用；类型放第二段。只定义已有消费者且用途稳定的资源，不提前填满档位。

| 类型 | 已实现的用途 |
| --- | --- |
| Color / Brush | Surface、Control、ControlHover、ControlPressed、Text、TextSecondary、TextMuted、Border、BorderInactive；Brush 另有 OnDanger |
| Radius | Small，沿用现有 5 DIP；其他圆角档位尚未确认 |
| Duration | Fast=80、Normal=160、Slow=220 ms |
| Easing | Standard、Decelerate、Spring；Spring 仍使用获准保留的 SukiUI |
| Opacity | Disabled，沿用 0.4 |
| Thickness | Focus，1 DIP 焦点轮廓，视觉待验收 |
| Text | Window.Close / Minimize / Maximize / Restore / Fullscreen / ExitFullscreen，可供本地化覆盖 |

Space.XS…XL、Radius.Medium/Large、Color.Danger 是命名方向，当前未定义数值或契约。不要把候选用途写成已经存在的能力。

控件级键仅用于具有独立覆盖需求的差异，例如 `Vhilz.CaptionButton.Close.Pressed.BackgroundBrush`、`Vhilz.CaptionButton.RevealBorder.Radius`。窗口与标题栏的局部画笔继续保留，以支持同一窗口内分别覆盖背景与前景。120 ms 染色与 400 ms 关闭按钮恢复时长保留为 `CaptionButton.Duration.Color`、`CaptionButton.Close.Duration.Reset`，不冒充通用档位。

Theme、Geometry 等资源保留明确角色结尾。Preview 只服务设计时，C# 入口为内部常量，不属于稳定应用契约。附加登记属性及 CaptionSurface 命名属性是内部实现，应用应覆盖控件公开属性、ControlTheme 或文档列出的资源。

## 动态覆盖与注释

- 非画笔值在消费属性上直接使用 DynamicResource；单个控件的差异通过 ControlTheme/样式 Setter 覆盖对应属性，不为相同数值建立控件别名。
- StaticResource 数值别名保留加载时的值，即使别名消费端使用 DynamicResource，也不会因原键被覆盖而重新解析；回归见 ResourceDictionaryTests。
- 语义画笔通过动态 Color 保持联动。应用级颜色覆盖位于 Application.Resources；局部差异优先覆盖画笔或属性。定义宿主与消费宿主的区别见 [资源字典设计](resource-dictionary-design.md)。
- C# 使用 `ResourceKeys.Color.Surface` 等常量；AXAML 使用对应完整键。变更时同步定义、引用、Demo 与测试，不拼接字符串。
- 注释解释宿主解析、状态更新顺序、平台边界或设计取舍；键名已经表达的含义不重复解释。

## 本轮迁移

不提供静态旧键别名；只覆盖旧键的应用必须更新。控件行为保持上游契约。

| 原键/用途 | 新键 |
| --- | --- |
| Vhilz.Global.Surface.Background.Color | Vhilz.Color.Surface |
| Vhilz.Global.Text.Primary/Secondary/Muted.Color | Vhilz.Color.Text / TextSecondary / TextMuted |
| Vhilz.Global.Surface.Border.Color | Vhilz.Color.Border |
| Vhilz.Global.Surface.Inactive.Border.Color | Vhilz.Color.BorderInactive |
| Vhilz.CaptionButton.CornerRadius | Vhilz.Radius.Small |
| Vhilz.CaptionButton.Disabled.Opacity | Vhilz.Opacity.Disabled |
| CaptionButton 表面变换时长 | Vhilz.Duration.Normal |
| CaptionButton 进入按下的颜色时长 | Vhilz.Duration.Fast |
| Window 全屏顶栏展开时长 / Win32 全屏过渡时长 | Vhilz.Duration.Normal / Slow |
| CaptionButton 通用染色时长 / 关闭按钮恢复时长 | Vhilz.CaptionButton.Duration.Color / Close.Duration.Reset |
| 通用颜色缓动 / 缓出 / 表面弹簧 | Vhilz.Easing.Standard / Decelerate / Spring |
| CaptionButton 常态 / 非激活前景 | Vhilz.Brush.TextSecondary / TextMuted |
| CaptionButton 关闭按钮交互前景 | Vhilz.Brush.OnDanger |
| CaptionButton.Motion.ParameterSelector.Converter | 删除；由 CaptionSurface 命名属性选择状态参数 |

验收入口和各阶段范围见 [优化任务清单](optimization-tasks.md)；验证结果由当次交付说明报告，本文不累积执行日志。
