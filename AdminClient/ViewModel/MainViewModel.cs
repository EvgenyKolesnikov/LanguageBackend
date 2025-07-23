using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using AdminClient.Commands;
using AdminClient.Options;
using Language.Model;
using Microsoft.Extensions.Options;

namespace AdminClient.ViewModel;

public class MainViewModel
{
    private readonly BackendOptions _options;
    private readonly HttpClient _httpClient;

    public ObservableCollection<BaseWord> BaseWords { get; set; } = new ();
    
    public ObservableCollection<string> ExtentedWords { get; set; } = new (){ "kek"};
    
    
    
    
    public TriggerCommand<object> OpenEditWordForm { get; set; }
    public TriggerCommand<object> ExtentedWordsCommand { get; set; }
    
    
    
    
    
    public MainViewModel(IOptions<BackendOptions> options,IHttpClientFactory clientFactory) 
    {
        _options = options.Value;
        _httpClient = clientFactory.CreateClient("HttpClient");
        InitializeCommands();
        InitializeData();
    }
    
    
    private async void InitializeData()
    {
        try
        {
            BaseWords = await GetWords();
        }
        catch (Exception e)
        {
            MessageBox.Show("Ошибка соединения с Сервером"); 
        }
    }

    private void InitializeCommands()
    {
        ExtentedWordsCommand = new TriggerCommand<object>(GetExtentedWords);
    }
    
  
    
    
    // Получить все слова
    private async Task<ObservableCollection<BaseWord>> GetWords()
    {
        var response = await _httpClient.GetAsync(_options.Host + "/api/Admin/Dictionary");
        var responseObj = await ResponseHandler.DeserializeAsync<ObservableCollection<BaseWord>>(response);
        return responseObj;
    }

    private async void GetExtentedWords(object word)
    {
        var Datacontext = ((Button)word).DataContext;
        if (Datacontext is BaseWord _baseWord)
        {
            var response = await _httpClient.GetAsync(_options.Host + $"/api/Admin/GetExtentedWord?baseWord={_baseWord.Word}");
            var responseObj = await ResponseHandler.DeserializeAsync<ObservableCollection<string>>(response);
            ExtentedWords.Clear();
            foreach (var item in responseObj)
            {
                ExtentedWords.Add(item);
            }
            
        }
    }
}