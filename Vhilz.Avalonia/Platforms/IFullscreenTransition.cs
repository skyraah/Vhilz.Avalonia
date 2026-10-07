namespace Vhilz.Avalonia.Theme.Platforms;

// 控件在上游提交状态或隐藏窗口之前通知平台实现，防止动画中间帧污染恢复矩形。
internal interface IFullscreenTransition : IDisposable {
    void BeforeStateChange();
    void BeforeHide();
}
