using Avalonia.Controls;
using TimeCanvas.ViewModels;

namespace TimeCanvas.Views;

public partial class TrendWindow : Window
{
    public TrendWindow(TrendViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.LoadCommand.ExecuteAsync(null);
    }
}
