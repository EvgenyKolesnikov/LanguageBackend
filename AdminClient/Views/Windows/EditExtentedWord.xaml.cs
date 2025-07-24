using System.Windows;
using AdminClient.ViewModel;

namespace AdminClient.Views.Windows;

public partial class EditExtentedWord : Window
{
    private MainViewModel _viewModel;
    
    public EditExtentedWord(MainViewModel viewModel)
    {
        _viewModel = viewModel;
        InitializeComponent();
        DataContext = _viewModel;
    }
    
    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}