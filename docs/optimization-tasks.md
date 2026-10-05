# 代码优化任务清单

本轮优先完成语义层迁移，并推进可独立落地的工程、窗口和可访问性改动。任务由资源/控件、工程/测试、窗口行为三个模块归属负责；当前在同一工作区顺序实现，避免并发改写共享字典。状态“已实现”表示源码与自动化准备完成，不代表用户视觉验收通过。

## 决策边界

- 已确认：`Vhilz.Color/Brush/Radius/Space/Duration/Easing/Opacity.*` 的语义层方向；Fast=80、Normal=160、Slow=220 ms，120/400 ms 保留为有明确用途的控件参数。圆角和间距沿用现值，不预设未确认档位。
- 已确认：只移除 Phosphor，保留 Lucide 和 SukiUI。因此本轮不替换图标库或弹簧缓动，也不自行选择 SukiUI 新版本；nightly 风险仍存在。
- 待用户确认：根命名空间是否从 `Vhilz.Avalonia` 调整。确认前保持现有 API 和 XAML xmlns，不自行实施破坏性迁移。
- 新增依赖、默认控件回退策略与扩大架构范围先讨论；已有确认继续有效。

## 第一阶段：语义资源与复用

| 任务/归属 | 改动目标与模块 | 验收标准 | 状态 |
| --- | --- | --- | --- |
| 语义层迁移 / 资源 | Tokens、Light/Dark、WindowDecorations、VhilzWindow 主题直接引用通用圆角、时长、缓动、透明度及语义颜色/画笔；仅保留独立覆盖的控件差异 | 明暗键及类型一致；运行时覆盖更新现有控件；原键消费者同步迁移 | 已实现 |
| 值别名行为 / 测试 | ResourceAliasProbe、ResourceDictionaryTests 先验证 StaticResource 别名，再验证应用级语义圆角/时长覆盖 | 别名保留加载值，直接 DynamicResource 更新；局部属性优先于共享 Token | 已实现，采用直接引用 |
| 键常量 / 资源 | ResourceKeys 集中所有项目字典键，C# 实现、Demo、库测试、Win32 冒烟查找使用常量 | C# 查找不再散落资源字符串；新增键同时更新常量；Preview 为内部入口 | 已实现 |
| 图标样式 / 控件 | WindowDecorations 的 CaptionButton.Icon.Theme 统一 Lucide 默认尺寸/描边；提取最大化尺寸与按钮组边距；公共白色前景只定义一次 | 生产模板与设计时预览复用样式；还原图标保留独立描边，现有图标测试通过 | 已实现，Demo 与设计时普通图标复用主题 |
| 命名与协作 / 文档 | AGENTS、resource-key-naming、resource-dictionary-design 采用短语义键，明确 Setter 覆盖、注释和公开/内部边界 | 旧规则标记废弃；文档与实现一致；尚未确认档位不写成规范 | 已实现 |

## 第二阶段：依赖与工程

| 任务/归属 | 改动目标与模块 | 验收标准 | 状态 |
| --- | --- | --- | --- |
| 依赖清理 / 工程 | 库 csproj 删除无引用 Phosphor；Directory.Packages.props 保持其余实际版本 | 编译与现有测试通过，直接依赖不再含 Phosphor | 已实现 |
| nightly 与图标耦合 / 控件 | SukiUI 弹簧替换、CaptionGeometryIcon 脱离 Lucide 的候选实现另行评估 | 用户授权后方可移除或替换；曲线、24 单位路径布局与明暗前景有回归 | 用户选择保留，本轮不执行 |
| 根命名空间 / 工程 | 若授权，统一库/Demo/测试 namespace、XmlnsDefinition 和消费迁移说明 | 清除框架名称遮蔽，保留已约定程序集/XAML URI，完整构建通过 | 待确认 |
| 构建配置 / 工程 | Directory.Build.props、中央包版本、editorconfig、CI、README、库打包元数据 | 构建属性单一来源；统一新增代码括号风格；CI 构建和测试，不隐式发布 | 已实现，远端 CI 待首次运行 |
| 测试项目边界 / 测试 | 将主题选择器用例移至 Demo.Tests，库测试只引用库；SmokeTests 加入解决方案 | 库测试可独立构建；Demo 用例继续验证真实选择器；冒烟项目可编译 | 已实现；冒烟仍引用 Demo，因为测试目标就是 Demo，未实际运行 |
| 文档整理 / 文档 | 设计规范移除日期式验证日志，旧诊断标为历史参考 | 现行规则有明确入口，调研不被当作当前实现承诺 | 已实现；其他历史日志按后续改动逐步归档 |

## 第三阶段：窗口实现与测试

| 任务/归属 | 改动目标与模块 | 验收标准 | 状态 |
| --- | --- | --- | --- |
| 装饰登记与避让 / 窗口 | DecorationRegistration 让自有模板主动登记按钮组；VhilzWindow 订阅 Bounds/Margin/可见性与窗口状态，移除 LayoutUpdated 树遍历 | 按钮尺寸、显隐、窗口状态改变仍正确避让；无按字符串搜索私有装饰树 | 已实现 |
| 全屏控制器 / 窗口 | FullscreenTitleBarController 承担顶边输入、浮层订阅、展开/收回与正文避让；VhilzWindow 保留上游属性、Esc 和 Win32 生命周期转交 | 明暗展开、反向、退出、隐藏以及内容先消费 Esc 的回归通过 | 已实现；延长浮层直接容器可见性仍是上游兼容边界，不能宣称完全摆脱私有结构 |
| 命名过渡参数 / 控件 | CaptionSurface.Motion 接收 TargetBackground、PressedBackground、DefaultDuration、PressedDuration、CloseResetDuration 等命名属性；删除位置转换器与两套 MultiBinding | 参数先于颜色动画目标更新；既有 Transition 实例接续颜色；覆盖前景时长有回归 | 已实现 |
| 揭示描边 / 控件 | CaptionSurface 无可见辉光时跳过指针位置重绘，强度/位置变化有像素影响时才失效 | 原有接近范围、退出和卸载回归通过；有效辉光移动仍重绘 | 已实现；有效帧的画笔分配后续按性能测量再优化 |
| 非激活绑定 / 控件 | 按钮组绑定一次继承的 IsInactive 附加属性，按钮主题读取继承状态 | 两种主题所有按钮共享状态，关闭按钮交互白色前景不被覆盖 | 已实现 |
| 确定性时钟 / 测试 | TestAnimationClock 统一全屏与按钮颜色测试，替代真实延时 | 精确推进中间帧及反向插值，覆盖时长更新，无 Task.Delay 计时依赖 | 已实现 |
| 测试可读性 / 测试 | 生产装饰模板搭建提取测试辅助入口，按钮状态大场景拆为主题/按钮参数化用例 | 每个状态失败可独立定位，辅助层不替代生产模板 | 已实现公共搭建入口与 12 个按钮/主题独立用例；单用例内按压与禁用仍按输入序列验证 |

## 第四阶段：可访问性

| 任务/归属 | 改动目标与模块 | 验收标准 | 状态 |
| --- | --- | --- | --- |
| 键盘焦点 / 控件 | CaptionButton 模板增加独立焦点轮廓，复用语义前景与描边参数；Demo 已有 Tab 导航预览 | 两种主题 focus-visible 结构回归通过；用户检查 Tab 轮廓对比度与缩放/禁用状态 | 已实现，视觉待验收 |
| 可访问名称 / 控件 | 生产按钮设置 AutomationProperties.Name，最大化时切换还原名称 | 默认名称非空，应用覆盖文字动态生效，屏幕阅读器实际结果由用户检查 | 已实现，系统阅读器未验证 |
| 本地化 / 资源 | Tokens 中的 Text.Window.* 统一名称与退出全屏提示 | 应用覆盖 ExitFullscreen 更新提示与名称；库模板不再写死中文提示 | 已实现 |

## 第五阶段：默认控件与回退

| 任务/归属 | 改动目标与模块 | 验收标准 | 状态 |
| --- | --- | --- | --- |
| 回退决策 / 资源 | 选择应用显式组合 Fluent，或库提供可选回退入口；VhilzTheme、Demo.App 与使用说明同步 | 依赖和主题加载顺序明确，不把未覆盖控件宣称为 Vhilz 原生支持 | 待讨论；README 说明现状和显式组合方式 |
| 第一批默认控件 / 控件 | 回退方案确认后，从 Button 开始，再到 TextBox/ComboBox；复用语义层，保持上游部件与键盘契约 | 每个控件有明暗与悬停、按下、禁用、焦点 Demo，构建及行为回归通过后用户验收 | 尚未实施，不属于本轮完成能力 |

## 用户复核入口

在仓库根目录执行（独立输出目录避免 Demo 占用原构建产物）：

```powershell
dotnet build Vhilz.Avalonia.sln --artifacts-path artifacts/optimization
dotnet test Vhilz.Avalonia.sln --artifacts-path artifacts/optimization
dotnet artifacts/optimization/bin/Vhilz.Avalonia.Demo/debug/Vhilz.Avalonia.Demo.dll
```

1. 切换明暗，Tab 导航按钮预览，检查轮廓、禁用、悬停、按下、回弹及快速反向的连续性。
2. 修改按钮显隐与窗口大小，检查标题栏避让；全屏顶边展开与收回时正文应同步，Esc 退出且内容可以先消费 Esc。
3. 非激活/重新激活、最大化/还原、不同 DPI 下检查标题、按钮图标和焦点。系统屏幕阅读器检查按钮用途。
4. 本地化示例：在应用 Resources 覆盖 `Vhilz.Text.Window.ExitFullscreen`；语义覆盖示例见 README。

Headless 验证结构/行为，不验证材质、模糊和最终动画观感。Win32 实际动画、系统阅读器以及 Linux/macOS 尚待实际验证；提交前先根据用户反馈修复。当前任务未授权 Git 提交、推送或发布。
