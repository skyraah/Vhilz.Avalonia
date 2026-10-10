using System;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Vhilz.Avalonia.Demo.Components.N01;

public partial class N01Sample : UserControl {
    private int _count;

    public N01Sample() {
        InitializeComponent();
        CommandButton.Command = new UnavailableCommand();
    }

    private void OnClick(object? sender, RoutedEventArgs e) {
        Status.Text = $"已执行 {++_count} 次";
    }

    private sealed class UnavailableCommand : ICommand {
        public bool CanExecute(object? parameter) => false;
        public void Execute(object? parameter) { }
        public event EventHandler? CanExecuteChanged { add { } remove { } }
    }
}
