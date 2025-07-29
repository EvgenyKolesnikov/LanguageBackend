using System.Windows;
using AdminClient.ViewModel;

namespace AdminClient.Views.Windows;

public partial class EditBaseWord : Window
{
    private MainViewModel _viewModel;
    
    public EditBaseWord(MainViewModel viewModel)
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