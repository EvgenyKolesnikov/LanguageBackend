using System.Windows.Controls;
using AdminClient.ViewModel;

namespace AdminClient.Views.Controls;

public partial class ExtentedWordsControl : UserControl
{
    private MainViewModel _viewModel;
    public ExtentedWordsControl(MainViewModel viewModel)
    {
        _viewModel = viewModel;
        InitializeComponent();
        DataContext = viewModel;
    }
}