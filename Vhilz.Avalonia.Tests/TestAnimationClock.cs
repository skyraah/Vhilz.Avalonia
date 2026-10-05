using System.Reflection;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Threading;

namespace Vhilz.Avalonia.Tests;

// 反射集中在测试边界；升级 Avalonia 时仅此处需要核对内部时钟入口。
internal sealed class TestAnimationClock {
    private readonly object _clock;
    private readonly MethodInfo _pulse;
    private double _milliseconds;

    public TestAnimationClock(params Animatable[] targets) {
        var type = typeof(Animatable).Assembly.GetType("Avalonia.Animation.ClockBase", true)!;
        _clock = Activator.CreateInstance(type, true)!;
        _pulse = type.GetMethod("Pulse", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var property = (AvaloniaProperty)typeof(Animatable)
            .GetField("ClockProperty", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
        foreach (var target in targets) target.SetValue(property, _clock);
    }

    public void AdvanceTo(double milliseconds) {
        _milliseconds = milliseconds;
        _pulse.Invoke(_clock, new object[] { TimeSpan.FromMilliseconds(milliseconds) });
        Dispatcher.UIThread.RunJobs();
    }

    public void AdvanceBy(double milliseconds) {
        // 第一脉冲建立本次动画起点，再精确推进到目标帧。
        AdvanceTo(_milliseconds);
        AdvanceTo(_milliseconds + milliseconds);
    }
}
