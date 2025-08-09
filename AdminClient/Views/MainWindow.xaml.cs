using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AdminClient.ViewModel;
using AdminClient.Views.Controls;

namespace AdminClient;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MainViewModel _viewModel;
    public string currentTag { get; set; }
    
    
    public MainWindow(MainViewModel viewModel)
    {
        _viewModel = viewModel;
        InitializeComponent();
        DataContext = _viewModel; // Добавляем DataContext
        Update();
    }
    
    public void Update()
    {
  
            
        switch (currentTag)
        {
            case ("Page1"):
                MainContentController.Content = new DictionaryControl(_viewModel);
                _viewModel.Update();
                break;
            default:
                MainContentController.Content = new DictionaryControl(_viewModel);
                ExtentedWordsController.Content = new ExtentedWordsControl(_viewModel);
                WordPropertiesController.Content = new WordPropertiesControl(_viewModel);
                TextContentController.Content = new TextControl(_viewModel);
                break;
        }
        UpdateLayout();
    }
    
    
    private void MenuClick(object sender, RoutedEventArgs e)
    {
        currentTag = (sender as Button).Tag.ToString();
        Update();
    }
}