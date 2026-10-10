using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Xunit;

namespace Vhilz.Avalonia.Tests.Components;

public class N26Tests
{
    public sealed record Row(string Name, string Value);

    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void ReadOnlyRowsKeepColumnBindingsAndSelection(bool light)
    {
        var rows = new[] { new Row("账单 1", "已完成"), new Row("账单 2", "处理中") };
        var table = new TableView { ItemsSource = rows, CanUserResizeColumns = true };
        table.Columns!.Add(new TableViewColumn { Header = "账单", Width = new GridLength(160), Binding = new Binding(nameof(Row.Name)) });
        table.Columns.Add(new TableViewColumn { Header = "状态", Width = new GridLength(160), Binding = new Binding(nameof(Row.Value)) });
        var window = new Window { Width = 500, Height = 300, Content = table,
            RequestedThemeVariant = light ? ThemeVariant.Light : ThemeVariant.Dark };
        window.Show();
        try
        {
            Assert.Equal(2, table.GetVisualDescendants().OfType<TableViewColumnHeader>().Count());
            Assert.Equal(4, table.GetVisualDescendants().OfType<TableViewCell>().Count());
            Assert.Contains(table.GetVisualDescendants().OfType<TableViewCell>(), x => Equals(x.Content, "账单 1"));
            Assert.Contains(table.GetVisualDescendants().OfType<TableViewCell>(), x => Equals(x.Content, "处理中"));
            table.SelectedIndex = 1;
            Assert.Same(rows[1], table.SelectedItem);
            table.ItemsSource = Array.Empty<Row>();
            window.UpdateLayout();
            Assert.Empty(table.GetVisualDescendants().OfType<TableViewRow>());
        }
        finally { window.Close(); }
    }
}
