# 当前版本：框线像素对齐试验

用户实际反馈：两种绘制方式无明显区别，前后窗间隙干净，模糊发生在框线本身。因此本次调整路径坐标和描边，不改变直接绘制管线。

默认 16 DIP 下，前框直边中心为 x=2.5/10.5、y=5.5/13.5；后框上边 y=2.5、右边 x=13.5。独立 Token `Vhilz.CaptionButton.Icon.Restore.StrokeWidth=1.5` 在 24 单位坐标系缩放后得到 1 DIP 描边。前后窗净间隙为 2 DIP，路径圆角半径仍为 1（显示为约 0.667 DIP）。由于边缘取整，前框中心线宽高由约 8.667 DIP 收至 8 DIP，整体轮廓也略收窄。

这是针对默认尺寸、100% 显示缩放且控件原点落在整像素时的对齐试验，并非全 DPI 动态吸附。125%、150% 或自定义尺寸下仍可能有部分像素覆盖，真实标题栏的布局原点也需结合 Demo 显示的位置核对。圆角继续保留抗锯齿。

验证：独立输出 `bin/RestorePixelCheck/` 构建 0 警告、0 错误；11 项测试通过。Skia 诊断 `--pixel-check` 在 16 DIP / 100% 下检查前框与后框的横竖直边，采样均为单像素 alpha=255，相邻像素 alpha=0。调整前同一行曾出现相邻两个 alpha=192 的像素。该证据仅证明上述离屏条件下的直边像素覆盖，真实视觉仍待用户验收。

运行 `Vhilz.Avalonia.Demo/bin/RestorePixelCheck/Vhilz.Avalonia.Demo.exe`，最大化后切换“像素对齐（默认）”与“调整前的路径与线宽”，在明暗主题下观察框线；第二项保留旧路径及旧描边用于比较。Demo 下方线宽数值描述的是默认生产图标。其他平台和真实窗口交互尚未自动化验证。

以下为前一阶段的历史诊断，旧路径、旧线宽及像素一致性结论不代表本次坐标调整后的状态。

---
# 窗口还原图标绘制核对（绘制管线对齐阶段记录）

## 结论与边界

已将窗口还原图标从 Viewbox → Canvas → Path 改为单控件直接绘制，沿用 Lucide 的布局与属性契约。路径仍为原先的双窗口形状，前后窗圆角半径均为 1，间隙未再次修改。

这完成了绘制方式对齐，但不能据此认定用户反馈的模糊已经消失。旧 Viewbox 和新实现的离屏结果都与 Lucide 一致；真实标题栏下的模糊原因仍需同位置对照验收。没有证据支持“Viewbox 必然先栅格化再放大”的解释。

## 核对来源

本机已安装 Lucide.Avalonia 0.2.24，目标程序集为 lib/net10.0/Lucide.Avalonia.dll，使用 ilspycmd 核对 LucideIcon、IconGeometryProvider 和 LucideIconExtensions。包记录上游提交为 `5b8419e5e8901b9840377d3564434fb419cc994a`，仓库为 <https://github.com/dme-compunet/Lucide.Avalonia>。当前项目使用 Avalonia 12.1.3。

确认的实现：

- Kind 从内部缓存取得 Geometry.Parse 后的路径，实际图标不经 SVG 图片控件或位图加载。
- MeasureOverride、ArrangeOverride 都返回 Size × Size。
- Render 先绘制透明边界；显式设置 Size 时，压入 Size / 24 的缩放矩阵，再 DrawGeometry(null, pen, geometry)。
- Pen 的 LineCap、LineJoin 均为 Round；描边随坐标系一起缩放。
- Size、StrokeWidth、Foreground、Kind 变化触发重绘，Size 也使测量失效。
- 公开 GetGeometryData() 可读取内置路径，但没有将自定义 Geometry 交给 LucideIcon 的公开入口。

内部 CaptionGeometryIcon 继承 LucideIcon，直接复用尺寸、测量、排列、前景色继承及相关属性失效逻辑，覆盖 Render 接收自定义路径。它保留动态路径资源、图标尺寸和描边 Token，没有新增公开控件 API。生产实现和 Demo 不反射访问 Lucide 私有字段。

## 已验证

在 Windows 开发环境运行：

```powershell
dotnet build Vhilz.Avalonia.sln --no-restore -p:OutputPath=bin/RestoreRenderCheck/
dotnet test Vhilz.Avalonia.Tests/Vhilz.Avalonia.Tests.csproj --no-build --no-restore -p:OutputPath=bin/RestoreRenderCheck/
```

解决方案构建 0 警告、0 错误；11 项测试通过。构建需要允许 Avalonia BuildServices 写入其本地日志，独立输出目录避免覆盖正在运行的旧 Demo。

测试核对窗口部件与绑定契约、动态路径和 Token 覆盖，以及自定义绘制器与已安装 Lucide 在相同 Square 路径下的实际绘制指令。尺寸覆盖 12、16、20、24，前景色覆盖黑白，描边覆盖 2 和 1.5，同时检查默认尺寸和空路径。

忽略目录 artifacts/restore-diagnostics 内的现有 Skia 工具也已扩展并运行：2 种路径 × 4 种尺寸 × 4 种 DPI，共 32 组新实现/Lucide 像素对照均无差异。该临时工具仅在诊断中反射注入相同路径，不用于生产。其现有 Bitmap.Save 调用有一条过时 API 警告。

这些检查不覆盖真实窗口的最终合成、实际屏幕观感或所有平台。

## Demo 验收入口

运行 `Vhilz.Avalonia.Demo/bin/RestoreRenderCheck/Vhilz.Avalonia.Demo.exe`，最大化窗口后使用新增下拉框：

| 选项 | 对照用途 |
| --- | --- |
| 还原图标：直接绘制 | 默认生产实现 |
| 还原图标：旧 Viewbox 绘制 | 同一路径、尺寸、前景色、描边和标题栏位置，观察绘制层级差异 |
| Square：自定义绘制器 | 从 Lucide 公开 API 读取 Square 路径，与还原图标使用相同尺寸 |
| Square：Lucide 原控件 | 与上一项核对自定义绘制器和原控件的真实标题栏效果 |

Demo 显示窗口 RenderScaling、图标尺寸、最终 DIP 线宽和两种实现的位置。Avalonia 12 的窗口装饰位于 TopLevelHost 中，与窗口是视觉兄弟；对照查找从共同视觉根开始。对照保留默认图标的布局占位，并随其可见状态切换。

先在当前屏幕缩放下切换前两项，观察前后窗缝隙是否变化；再切换后两项，观察相同 Square 是否一致。分别切换浅色、深色，检查悬停、按下和窗口还原操作。如果移动到不同缩放的显示器，记录 Demo 显示的缩放值与坐标。

普通窗口里的最大化 Square 仍为 12 DIP，对应 1 DIP 描边；默认还原图标及同尺寸 Square 对照为 16 DIP，对应约 1.333 DIP 描边。100% 离屏采样中，Square 12 的直边可以落在单个满覆盖像素上，Square 16 会分配到相邻像素；这证明尺寸与像素覆盖存在差异，尚不能证明它是用户实际模糊的唯一原因。

真实窗口视觉仍待用户验收；本轮未执行原生窗口交互自动化，未验证 Linux、macOS 或跨显示器效果。
