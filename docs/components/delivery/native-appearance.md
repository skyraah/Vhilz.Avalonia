# 原生组件首轮外观交付

日期：2026-10-09。主工作区为 `C:\project\Vhilz.Avalonia`，基线为 `49b11971b311859309e0fb926bc83617c67cfa09`。
保留任务开始时已有的按钮、窗口、资源、文档与测试未提交改动；本轮未执行 Git 提交、推送或发布。

## 实际范围

F00/F01 已接入目录宿主和共享资源；N01–N28 首轮原生外观，以及无需新增包的 E01–E03、E05–E14 已集成。
生产主题入口包含 79 个控件字典，Demo 显式登记 41 个 Sample。各任务文件归属、部件核对和状态演示见本目录对应任务交付包，
实际状态统一记录在 [领取表](../claims.md)。

参考 [shadcn 原版分析](../shadcn-style-reference.md) 的中性色、细边框、选中与焦点层次，保留既有 5 DIP 圆角、32 DIP 控件高度和 0.4 禁用透明度。
普通控件使用简单表面，弹层使用轻阴影；没有扩大玻璃材质范围、引入主题回退依赖或新增控件公共属性。
新增共享资源和覆盖方式见 [F01](F01.md) 与 [资源命名规范](../../resource-key-naming.md)。

以下范围仍未实施，不计为整卡或全计划验收：

- N11：完成 ProgressBar 首轮；Spinner 后续里程碑未实施。
- N26：完成 TableView 只读第一里程碑；编辑、排序等数据交互未实施。
- E04 ColorPicker、E15 DataGrid：独立包与依赖决策未实施。
- C 系列组合/自研组件：留待原生基件稳定后的后续建设。

## 集成与自检修正

主树已统一主题加载顺序和资源常量，独立自检发现的问题也已修正：

- 文字控件保留前景继承，避免覆盖按钮和选中项的逆色文字；输入错误边框与焦点环可以同时显示。
- ToggleButton 分开选中与未选中悬停/按下外观；ToggleSwitch 圆点行程按原生 Canvas 位移契约设置。
- ScrollBar 页按钮使用透明独立辅助主题；ButtonSpinner 半格按钮可完整点击；PipsPager 的小按钮不再被普通按钮最小高度撑大。
- ProgressBar 文字有独立表面底板；PathIcon 提供原生通知和导航图标的必要显示主题；DrawerPage 提示改为本地化资源。
- E08 示例包装滚动容器前先解除内容父级；N24 示例显式管理 AdornerLayer 与通知宿主，卸载时关闭通知并清理实例。

本轮没有新增视觉动画曲线。保留既有 ButtonMotion、窗口动效，以及 ProgressBar 不确定进度、Refresh 的必要原生运动。
NotificationCard 保留 1 ms 的非视觉 IsClosed 完成调度，避免 CloseAll 枚举时同步删除集合；这不是待打磨的视觉动画。

## 实际验证

最终解决方案构建通过，0 警告、0 错误：

```powershell
dotnet build Vhilz.Avalonia.sln --artifacts-path artifacts/agent
```

库的 38 项聚焦测试通过，涵盖原生主题、明暗资源键/类型和动态覆盖、Button、N24、N25、N26、E13。
Demo 的 2 项目录回归通过，分别在明暗主题下加载并替换全部 41 个 Sample：

```powershell
dotnet test Vhilz.Avalonia.Tests/Vhilz.Avalonia.Tests.csproj --artifacts-path artifacts/agent --filter 'FullyQualifiedName~NativeAppearanceTests|FullyQualifiedName~ResourceDictionaryTests|FullyQualifiedName~ButtonTests|FullyQualifiedName~N24Tests|FullyQualifiedName~N25Tests|FullyQualifiedName~N26Tests|FullyQualifiedName~E13Tests' --no-build --no-restore
dotnet test Vhilz.Avalonia.Demo.Tests/Vhilz.Avalonia.Demo.Tests.csproj --artifacts-path artifacts/agent --filter FullyQualifiedName~ComponentCatalogTests --no-build --no-restore
```

这些结果只证明对应结构与行为，不证明视觉已验收；未运行全解决方案全部测试。
上游模板和参考来源的许可已登记到根目录 `THIRD-PARTY-NOTICES.md`，随库打包。

## 用户观察入口

```powershell
dotnet artifacts/agent/bin/Vhilz.Avalonia.Demo/debug/Vhilz.Avalonia.Demo.dll
```

主窗口点击“打开原生组件外观目录”，切换明暗主题并选择 Sample，观察适用的悬停、按下、禁用、焦点、选中和校验外观。
重点观察输入焦点与错误的并存、选中项文字对比、弹层边界和阴影、滚动条与滑块尺寸、日期日历布局、表格密度及 RTL。
动效由用户继续打磨。本轮尚未实机验证视觉、Linux/macOS、真实触摸、IME/剪贴板或不同缩放比例。
