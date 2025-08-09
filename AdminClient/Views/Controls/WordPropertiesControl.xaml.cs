using System.Windows.Controls;
using AdminClient.ViewModel;

namespace AdminClient.Views.Controls;

public partial class WordPropertiesControl : UserControl
{
    private MainViewModel _viewModel;
    
    public WordPropertiesControl(MainViewModel viewModel)
    {
        _viewModel = viewModel;
        InitializeComponent();
        DataContext = viewModel;
    }
}