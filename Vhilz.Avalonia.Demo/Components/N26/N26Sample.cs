using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using static Vhilz.Avalonia.Demo.Components.NativeSampleLayout;

namespace Vhilz.Avalonia.Demo.Components.N26;

public sealed class N26Sample : UserControl
{
    public N26Sample()
    {
        var rows = Enumerable.Range(1, 80).Select(i => new Row($"INV-{i:000}", i % 3 == 0 ? "处理中" : "已完成", $"¥ {i * 120:N0}", i % 7 == 0 ? "这是一段较长的付款说明，用于观察单元格裁切与列宽调整。" : "团队订阅")).ToArray();
        var table = new TableView { Height = 300, ItemsSource = rows, CanUserResizeColumns = true };
        table.Columns!.Add(new TableViewColumn { Header = "账单", Width = new GridLength(130), Binding = new Binding(nameof(Row.Invoice)) });
        table.Columns.Add(new TableViewColumn { Header = "状态", Width = new GridLength(130), Binding = new Binding(nameof(Row.Status)) });
        table.Columns.Add(new TableViewColumn { Header = "金额", Width = new GridLength(130), Binding = new Binding(nameof(Row.Amount)), HorizontalContentAlignment = HorizontalAlignment.Right });
        table.Columns.Add(new TableViewColumn { Header = "说明", Width = new GridLength(360), Binding = new Binding(nameof(Row.Description)) });
        var status = Text("80 行 · 可调整列宽和原生行选择 · 首轮只读");
        table.SelectionChanged += (_, _) => status.Text = table.SelectedItem is Row row ? $"已选择 {row.Invoice}" : "尚未选择行";
        Content = Column(Text("TableView · 只读账单表"), table, status,
            Action("空数据", () => table.ItemsSource = Array.Empty<Row>()),
            Action("3 行", () => table.ItemsSource = rows.Take(3).ToArray()),
            Action("恢复 80 行", () => table.ItemsSource = rows));
    }

    public sealed record Row(string Invoice, string Status, string Amount, string Description);

}
