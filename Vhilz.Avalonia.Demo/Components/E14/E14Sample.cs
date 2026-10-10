using Avalonia.Controls;
using static Vhilz.Avalonia.Demo.Components.NativeSampleLayout;

namespace Vhilz.Avalonia.Demo.Components.E14;

public sealed class E14Sample : UserControl
{
    public E14Sample()
    {
        var status = Text("触摸设备可在滚动顶端向下拉动；按钮仅演示程序化刷新。");
        var refresh = new RefreshContainer { Height = 260, Content = new ScrollViewer { Content = Column(Enumerable.Range(1, 18).Select(i => (Control)Text($"活动条目 {i:00}")).ToArray()) } };
        var count = 0;
        refresh.RefreshRequested += async (_, e) =>
        {
            var deferral = e.GetDeferral();
            status.Text = "正在刷新…";
            try { await Task.Delay(900); status.Text = $"刷新完成 · 第 {++count} 次"; }
            finally { deferral.Complete(); }
        };
        Content = Column(Text("RefreshContainer · 原生刷新与完成机制"), refresh, Action("程序化请求刷新", () => refresh.RequestRefresh()), status);
    }

}
