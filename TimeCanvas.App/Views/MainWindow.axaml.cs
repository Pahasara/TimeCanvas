using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Microsoft.Extensions.DependencyInjection;
using TimeCanvas.ViewModels;

namespace TimeCanvas.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        viewModel.SettingsRequested += async (_, _) =>
        {
            var window = Program.Services.GetRequiredService<SettingsWindow>();
            await window.ShowDialog(this);
        };

        viewModel.HistoryRequested += async (_, _) =>
        {
            var window = Program.Services.GetRequiredService<TrendWindow>();
            await window.ShowDialog(this);
        };

        viewModel.TemplatesRequested += async (_, _) =>
        {
            var window = Program.Services.GetRequiredService<TemplateEditorWindow>();
            await window.ShowDialog(this);
        };

        Loaded += async (_, _) => await viewModel.LoadCommand.ExecuteAsync(null);
    }

    private void TaskDescription_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter || sender is not Control control) return;

        var window = control.FindAncestorOfType<Window>() ?? this;
        window.Focus();
        e.Handled = true;
    }
}
