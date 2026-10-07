# Agent 工作约束

本文适用于整个仓库；用户在当前任务中的明确要求优先。约束以实际实现和现有配置为准。

## 项目边界

- Vhilz.Avalonia 是基于 Avalonia 和 Fluid.Avalonia.Acrylic 的主题库，报告能力时以当前实现为准。
- 保留 Avalonia 的公开 API 和交互契约。重写模板时核对模板部件、状态、绑定、键盘和焦点行为。
- 视觉以克制的玻璃材质和 Linear 风格为方向；玻璃优先用于确有层次需求的窗口或弹层。方案只依赖应用内视觉树内容，并保持可维护的跨平台行为。

## 主题与资源

- 颜色、圆角、间距、阴影、透明度、动画时长和缓动使用语义 Token；稳定后再将适合复用的局部参数提取到公共 Token。
- 跨主题参数放在 `Vhilz.Avalonia/Themes/Resources/Tokens.axaml`，明暗差异分别放在 `Light.axaml` 和 `Dark.axaml`，语义键保持对应。
- 资源键使用 `Vhilz.<类型>.<用途>`；C# 资源查找使用 `ResourceKeys`，AXAML 与常量同步。需要主题切换或应用覆盖时使用支持动态更新的引用。
- 编辑 AXAML 选择器、Classes、嵌套 Style、ControlTheme 或状态切换时，先读 [选择器与 Classes 执行规范](docs/components/selector-class-rules.md)；语法不确定时再查 [编写指南与速查](docs/components/selector-class-guide.md)。资源命名细则见 [资源键命名](docs/resource-key-naming.md)。

## 工作范围

- 实施组件建设任务时，读取 [组件建设表](docs/components/README.md) 和对应任务卡，并遵守其中的文件归属与交付约定。
- 在已授权范围内自主完成实现、局部重构、修复和验证。新增依赖、公共 API、现有交互行为或大范围架构调整先讨论；已有明确授权时继续执行。
- 对低风险、非破坏性且可逆的实现选择，基于合理假设直接推进并完成适用验证；仅在信息缺失会影响公开 API、依赖或架构、交付范围，或造成明显返工时提问。
- Git 提交、推送和发布仅在用户明确要求后执行。

## 验证与交付

- 代码或主题改动完成前通过构建；行为或回归风险使用相关的聚焦测试。低风险样式、文档或简单修复可跳过自动化测试，并在交付中说明。
- 选定范围的验证通过后停止；后续改动、失败或明确风险再补充验证。
- 新增控件或明显视觉、交互改动提供 Demo，覆盖明暗主题和适用的悬停、按下、禁用、焦点状态。Headless 结果只说明结构或行为；视觉效果由用户实际观察确认。
- 交付时说明实际构建和测试、待用户观察的视觉项目，以及未验证的平台或效果。仅修改文档时检查内容和格式即可。

常用入口：

```powershell
dotnet build Vhilz.Avalonia.sln --artifacts-path artifacts/agent
dotnet test Vhilz.Avalonia.sln --artifacts-path artifacts/agent
dotnet artifacts/agent/bin/Vhilz.Avalonia.Demo/debug/Vhilz.Avalonia.Demo.dll
```

## 语言与代码

- Agent 解释、项目文档和代码注释使用中文；代码标识符和资源键使用英文。
- 注释说明设计原因、平台限制或上游兼容处理，避免复述代码。
- 代码格式遵循仓库现有配置；依赖版本、项目结构和命令以项目文件与实际环境为准。
