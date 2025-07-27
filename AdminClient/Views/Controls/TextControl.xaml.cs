using System.Windows.Controls;
using AdminClient.ViewModel;

namespace AdminClient.Views.Controls;

public partial class TextControl : UserControl
{
    private MainViewModel _viewModel;
    public TextControl(MainViewModel viewModel)
    {
        _viewModel = viewModel;
        InitializeComponent();
        DataContext = viewModel;
    }
}