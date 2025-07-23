using System.Windows.Controls;
using AdminClient.ViewModel;

namespace AdminClient.Views.Controls;

public partial class DictionaryControl : UserControl
{
    private MainViewModel _viewModel;

    public DictionaryControl(MainViewModel viewModel)
    {
        _viewModel = viewModel;
        InitializeComponent();
        DataContext = viewModel;
    }
}