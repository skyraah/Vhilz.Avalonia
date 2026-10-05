# Vhilz.Avalonia

基于 Avalonia 与 Fluid.Avalonia.Acrylic 的主题库，视觉方向为克制的玻璃层次与简洁表面。当前实现 VhilzWindow、TitleBar 和窗口装饰按钮，尚未覆盖全部默认控件。材质采样限于应用视觉树，不依赖桌面背景或真实折射。

## 使用

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:vhz="https://github.com/skyraah/Vhilz.Avalonia">
    <Application.Styles>
        <vhz:VhilzTheme />
    </Application.Styles>
</Application>
```

窗口使用 `vhz:VhilzWindow`。`TextBox`、`ComboBox` 等尚无 Vhilz 模板；需要这些默认控件的应用自行引用 Avalonia.Themes.Fluent，在 VhilzTheme 之前加载 FluentTheme，或像 Demo 一样在工具容器中局部加载。库目前不自动引入回退主题。

## 覆盖语义资源

```xml
<Application.Resources>
    <Color x:Key="Vhilz.Color.Surface">#202024</Color>
    <CornerRadius x:Key="Vhilz.Radius.Small">6</CornerRadius>
    <x:TimeSpan x:Key="Vhilz.Duration.Fast">0:0:0.09</x:TimeSpan>
    <x:String x:Key="Vhilz.Text.Window.ExitFullscreen">Exit fullscreen (Esc)</x:String>
</Application.Resources>
```

以上片段需要 `xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"`。在明暗 ThemeDictionaries 中用同一键可分别覆盖；单控件差异优先设置公开属性或样式 Setter。C# 查找使用 `Vhilz.Avalonia.Theme.ResourceKeys`。完整命名和迁移规则见 [资源键命名](docs/resource-key-naming.md)。

## 构建与验收

```powershell
dotnet build Vhilz.Avalonia.sln --artifacts-path artifacts/optimization
dotnet test Vhilz.Avalonia.sln --artifacts-path artifacts/optimization
dotnet artifacts/optimization/bin/Vhilz.Avalonia.Demo/debug/Vhilz.Avalonia.Demo.dll
```

库测试与 Demo 集成测试分开，Win32 冒烟程序作为独立可执行项目加入解决方案，普通 dotnet test 不会运行它。CI 构建与测试不发布包。包版本以 Directory.Packages.props 为准；SukiUI 与 Lucide 按用户选择保留，SukiUI 当前为 nightly。

[五阶段优化任务与用户检查步骤](docs/optimization-tasks.md)区分已实现、待确认与待视觉验收事项。主题尚在建设中，Headless 通过不代表视觉通过。
