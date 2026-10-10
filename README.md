# Vhilz.Avalonia

基于 Avalonia 与 Fluid.Avalonia.Acrylic 的主题库，视觉方向为克制的玻璃层次与简洁表面。当前提供 VhilzWindow、TitleBar、GeometryIcon、SvgIcon、窗口装饰及计划内原生控件的首轮主题。原生外观参考 shadcn/ui，视觉待用户验收；材质采样限于应用视觉树。

## 使用

组件建设与协作入口见 [组件建设与领取表](docs/components/README.md)：按 shadcn/ui 目录组织，优先覆盖 Avalonia 原生控件；每项都有依赖、文件归属和验收任务卡。

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:vhz="https://github.com/skyraah/Vhilz.Avalonia">
    <Application.Styles>
        <vhz:VhilzTheme />
    </Application.Styles>
</Application>
```

窗口使用 `vhz:VhilzWindow`。`Button`、输入、选择、滚动、菜单、日期时间、树、TableView、通知及页面家族等原生主题随 `VhilzTheme` 加载，库不自动引入 Fluent/Simple 回退。

Demo 的“打开原生组件外观目录”提供 41 个独立样例，可切换明暗并执行真实操作。范围、限制与验证见 [原生外观交付](docs/components/delivery/native-appearance.md)，样式分析见 [shadcn 原版参考](docs/components/shadcn-style-reference.md)。Spinner 后续里程碑、ColorPicker/DataGrid 独立包和 C 系列组合/自研组件尚未实施。已有按钮/窗口动效保留，新增外观过渡由用户继续打磨。

## 基础 Button

加载 `VhilzTheme` 后，原生 `Button` 默认使用 Vhilz 基础模板；在局部 Fluent 容器内可以显式指定主题：

```xml
<Button Content="确认" />
<Button Content="确认" Theme="{DynamicResource Vhilz.Button.Theme}" />
```

按下动效通过 `ButtonMotion.PressAnimation` 选择，默认 `None`；`Scale` 复用标题栏按钮的缩放幅度、时长和弹簧缓动：

```xml
<Button Content="确认" vhz:ButtonMotion.PressAnimation="Scale" />
```

也可在 C# 中使用 `ButtonMotion.SetPressAnimation(button, ButtonPressAnimation.Scale)`，运行时改值会立即切换。缩放只作用于内容表面，不改变按钮的命中区域。

模板支持文本、图文和 `ContentTemplate`，保留原生点击、命令、访问键及默认/取消按钮行为。明暗表面包含悬停、按下、弹层展开、禁用和键盘焦点状态；`IsDefault` 使用更清晰的描边。表面颜色使用 `Vhilz.Color.Control`、`ControlHover`、`ControlPressed` 及对应的 `Brush` 资源。显式设置 `Background` 会保持该背景，不再应用默认交互状态色。

圆角、禁用透明度、背景过渡分别复用 `Vhilz.Radius.Small`、`Vhilz.Opacity.Disabled` 和 `Vhilz.Duration.Fast`；基础内边距与最小高度暂由 Button 主题 Setter 定义，不代表全局密度规范。Demo 顶部“基础 Button”和 `Button.axaml` 的设计预览提供检查入口，颜色、焦点轮廓与动画观感待实际验收。

## 自定义路径图标

`vhz:GeometryIcon` 是公开的 SVG 路径控件，位于 `Vhilz.Avalonia.Theme.Controls`：

```xml
<vhz:GeometryIcon Data="M4 12h16 M12 4v16"
                  Size="24" StrokeWidth="2"
                  Foreground="{DynamicResource Vhilz.Brush.Text}" />
```

`Data` 支持 XAML 路径字符串、`Geometry` 对象及动态资源；设为 `null` 时不绘制。路径使用 24×24 坐标，`Size` 同时缩放路径和描边；圆角描边、不填充。控件继承 Lucide 的尺寸、布局与前景色规则，放入按钮时可以继承其前景色；继承的 `Kind` 不参与绘制。窗口还原图标也使用此组件。

## SVG 文件图标

`vhz:SvgIcon` 使用 Svg.Controls.Skia.Avalonia 加载 SVG，本地文件和 Avalonia 资源均通过 `Path` 指定：

```xml
<vhz:SvgIcon Path="avares://MyApp/Assets/Icons/layers.svg"
             Size="24" Foreground="{DynamicResource Vhilz.Brush.Text}" />
<vhz:SvgIcon Path="C:/Icons/layers.svg" Size="32" />
```

资源文件需在应用项目中配置为 `AvaloniaResource`，URI 中的 `MyApp` 替换为实际程序集名。推荐使用完整 `avares://` URI；C# 中可用 `new SvgIcon(baseUri)` 为相对资源路径提供基准 URI。本地相对文件路径按进程工作目录解析。

`Size` 默认为 24 DIP，布局为正方形，默认 `Stretch.Uniform` 保留 SVG 的 `viewBox` 比例。SVG 自身控制填充和描边，不使用 `GeometryIcon.StrokeWidth`。固定颜色会保留；`fill="currentColor"` 或 `stroke="currentColor"` 默认跟随 `Foreground` 的纯色值，支持继承按钮前景色与主题切换。非纯色画笔回退为黑色，画笔透明度不映射到 SVG 颜色；整体透明度使用控件的 `Opacity`。可显式设置 `CurrentColor` 覆盖默认颜色绑定。

清空 `Path` 会清空文件图标；文件缺失或解析失败沿用上游的日志与空内容行为。继承的 `Source` 支持 SVG XML，避免同时设置 `Path` 与 `Source`。SVG 语法、滤镜及动画支持以上游为准，不承诺完整浏览器 SVG 能力。该版本依赖保持项目当前的 Avalonia 与 SkiaSharp 版本。

Demo 顶部的“自定义路径图标”和“SVG 文件图标”提供独立、按钮内、禁用及多色示例。切换浅色/深色并检查悬停、按下、焦点、非正方形 SVG 比例；最终视觉待用户验收。

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

## 平台代码组织

- `Vhilz.Avalonia/Controls/Windows` 存放跨平台窗口控件与标题栏逻辑；这里的 Windows 指窗口集合。
- `Vhilz.Avalonia/Platforms` 存放内部平台接口与选择入口，控件通过该入口接入平台功能。
- `Vhilz.Avalonia/Platforms/Windows` 存放 Windows 专属全屏过渡、Win32 消息钩子和原生调用；非 Windows 或 Headless 后端不挂载该实现。
- `Vhilz.Avalonia.Demo/Platforms/Windows` 存放 Windows 应用清单，`Platforms/X11` 存放 X11 启动配置。
- `Vhilz.Avalonia.Win32.SmokeTests` 是独立的 Windows 原生回归项目，跨平台测试保留在库与 Demo 各自的测试项目中。

单平台实现放入对应的 `Platforms/<平台>` 目录，与通用控件、平台接口和启动入口分开；平台类型使用内部命名空间，公开控件 API 保持不变。

## 构建与验收

```powershell
dotnet build Vhilz.Avalonia.sln --artifacts-path artifacts/agent
dotnet test Vhilz.Avalonia.sln --artifacts-path artifacts/agent
dotnet artifacts/agent/bin/Vhilz.Avalonia.Demo/debug/Vhilz.Avalonia.Demo.dll
```

库测试与 Demo 集成测试分开，Win32 冒烟程序作为独立可执行项目加入解决方案，普通 dotnet test 不会运行它。CI 构建与测试不发布包。包版本以 Directory.Packages.props 为准；SukiUI 与 Lucide 按用户选择保留，SukiUI 当前为 nightly。

[五阶段优化任务与用户检查步骤](docs/optimization-tasks.md)区分已实现、待确认与待视觉验收事项。主题尚在建设中，Headless 通过不代表视觉通过。
