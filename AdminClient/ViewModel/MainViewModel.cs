using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using AdminClient.Commands;
using AdminClient.Options;
using Language.Model;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Net.Http.Json;
using AdminClient.Views.Windows;
using Language.Dictionary.Requests;
using Language.Dictionary.Responses;

namespace AdminClient.ViewModel;

public class MainViewModel : BaseViewModel
{
    private readonly BackendOptions _options;
    private readonly HttpClient _httpClient;

    public ObservableCollection<BaseWord> BaseWords { get; set; } = new ();
    
    public ObservableCollection<GetExtentedWords> ExtentedWords { get; set; } = new ();
    
    public string NewExtentedWord { get; set; }
    public TriggerCommand<object> SaveExtentedWordCommand { get; set; }
    
    
    
    
    public TriggerCommand<object> OpenEditWordForm { get; set; }
    public TriggerCommand<object> ExtentedWordsCommand { get; set; }
    public TriggerCommand AddExtentedWordCommand { get; set; }
    public TriggerCommand<object> OpenEditExtentedWordForm { get; set; }
    public TriggerCommand EditExtentedWordCommand { get; set; }
    public TriggerCommand<object> DeleteExtentedWordCommand { get; set; }
    
    
    public AddExtentedWord AddExtentedWordRequest { get; set; } = new();
    public EditExtentedWordRequest EditExtentedWordRequest { get; set; } = new();
    
    public MainViewModel(IOptions<BackendOptions> options,IHttpClientFactory clientFactory) 
    {
        _options = options.Value;
        _httpClient = clientFactory.CreateClient("HttpClient");
        InitializeCommands();
        InitializeData();
    }

    public void Update()
    {
        InitializeData();
    }
    
    private async void InitializeData()
    {
        try
        {
            BaseWords = await GetWords();
            RaisePropertyChanged(nameof(BaseWords));
        }
        catch (Exception e)
        {
            MessageBox.Show("Ошибка соединения с Сервером"); 
        }
    }

    private void InitializeCommands()
    {
        ExtentedWordsCommand = new TriggerCommand<object>(GetExtentedWords);
        AddExtentedWordCommand =  new TriggerCommand(AddExtentedWord);
        OpenEditExtentedWordForm = new TriggerCommand<object>(EditExtentedWordForm);
        EditExtentedWordCommand = new TriggerCommand(EditExtentedWord);
        DeleteExtentedWordCommand =  new TriggerCommand<object>(DeleteExtentedWord);
    }
    
  
    
    
    // Получить все слова
    private async Task<ObservableCollection<BaseWord>> GetWords()
    {
        var response = await _httpClient.GetAsync(_options.Host + "/api/Admin/Dictionary");
        var responseObj = await ResponseHandler.DeserializeAsync<ObservableCollection<BaseWord>>(response);
        
        return responseObj;
    }

    // удалить расширенное слово
    private async void DeleteExtentedWord(object word)
    {
        var Datacontext = ((Button)word).DataContext;
        if (Datacontext is GetExtentedWords _extentedWord)
        {
            var response = await _httpClient.DeleteAsync(_options.Host + $"/api/Admin/ExtentedWord/{_extentedWord.Id}");
            if (response.IsSuccessStatusCode)
            {
                ExtentedWords.Remove(_extentedWord);
            }
        }
    }
    
    // Получить все расширенные слова
    private async void GetExtentedWords(object word)
    {
        var Datacontext = ((Button)word).DataContext;
        if (Datacontext is BaseWord _baseWord)
        {
            var response = await _httpClient.GetAsync(_options.Host + $"/api/Admin/GetExtentedWord?baseWord={_baseWord.Word}");
            var responseObj = await ResponseHandler.DeserializeAsync<ObservableCollection<GetExtentedWords>>(response);
            ExtentedWords.Clear();
            foreach (var item in responseObj)
            {
                ExtentedWords.Add(item);
            }
            
            AddExtentedWordRequest.BaseIdWord = _baseWord.Id;
        }
    }

    // добавить расширенное слово
    private async void AddExtentedWord()
    {
        var response = await _httpClient.PostAsJsonAsync(_options.Host + "/api/Admin/ExtentedWord", AddExtentedWordRequest);
        
        if (response.IsSuccessStatusCode)
        {
            var responseObj = await ResponseHandler.DeserializeAsync<GetExtentedWords>(response);
            ExtentedWords.Add(responseObj);
        }
    }

    private async void EditExtentedWordForm(object word)
    {
        var Datacontext = ((Button)word).DataContext;
        if(Datacontext is GetExtentedWords _word) // rework
        {
            EditExtentedWordRequest.Id = _word.Id;
            EditExtentedWordRequest.BaseWordId = _word.BaseWordId;
            EditExtentedWordRequest.Word = _word.Word;
        }

       
        var win = new EditExtentedWord(this);
        win.Show();  
    }

    // Редактировать расширенное слово
    private async void EditExtentedWord()
    {
        var response = await _httpClient.PutAsJsonAsync(_options.Host + $"/api/Admin/ExtentedWord/{EditExtentedWordRequest.Id}", EditExtentedWordRequest);
        if (response.IsSuccessStatusCode)
        {
            var responseObj = await ResponseHandler.DeserializeAsync<GetExtentedWords>(response);
            
            var objToEdit = ExtentedWords.FirstOrDefault(i => i.Id == responseObj.Id);
            
            if (objToEdit != null)
            {
                int i = ExtentedWords.IndexOf(objToEdit);
                ExtentedWords[i] = responseObj;
            }
        }
    }
    

    // Для обновления UI при изменении NewExtentedWord
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}