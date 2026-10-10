using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using static Vhilz.Avalonia.Demo.Components.NativeSampleLayout;

namespace Vhilz.Avalonia.Demo.Components.N23;

public sealed class N23Sample : UserControl
{
    public N23Sample()
    {
        var carousel = new Carousel { Height = 180, ItemsSource = new[] { "设计系统", "协同工作", "发布记录" }, SelectedIndex = 0,
            ItemTemplate = new FuncDataTemplate<string>((value, _) => new Border { Padding = new Thickness(24), Child = Text(value ?? "") }) };
        var status = Text("第 1 / 3 项");
        var previous = Action("← 上一项", () => carousel.Previous());
        var next = Action("下一项 →", () => carousel.Next());
        void Update()
        {
            previous.IsEnabled = carousel.SelectedIndex > 0;
            next.IsEnabled = carousel.SelectedIndex < carousel.ItemCount - 1;
            status.Text = carousel.ItemCount == 0 ? "空集合" : $"第 {carousel.SelectedIndex + 1} / {carousel.ItemCount} 项";
        }
        carousel.PropertyChanged += (_, e) => { if (e.Property == Carousel.SelectedIndexProperty || e.Property == Carousel.ItemCountProperty) Update(); };
        Update();
        Content = Column(Text("原生 Carousel · 前后导航与集合边界"), carousel, new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Children = { previous, next } }, status,
            Action("空集合", () => { carousel.ItemsSource = Array.Empty<string>(); Update(); }),
            Action("单项", () => { carousel.ItemsSource = new[] { "只有一项" }; carousel.SelectedIndex = 0; Update(); }),
            Action("恢复多项", () => { carousel.ItemsSource = new[] { "设计系统", "协同工作", "发布记录" }; carousel.SelectedIndex = 0; Update(); }));
    }

}
