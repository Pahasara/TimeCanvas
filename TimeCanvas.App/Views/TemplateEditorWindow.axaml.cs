using Avalonia.Controls;
using TimeCanvas.ViewModels;

namespace TimeCanvas.Views;

public partial class TemplateEditorWindow : Window
{
    public TemplateEditorWindow(TemplateEditorViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.LoadCommand.ExecuteAsync(null);
    }
}
