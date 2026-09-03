using Avalonia.Controls;
using TimeCanvas.ViewModels;

namespace TimeCanvas.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow(SettingsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.Saved += (_, _) => Close();
        Loaded += async (_, _) => await viewModel.LoadCommand.ExecuteAsync(null);
    }
}
