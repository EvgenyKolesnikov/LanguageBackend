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
using Language.Dictionary;
using Language.Dictionary.Requests;
using Language.Dictionary.Responses;

namespace AdminClient.ViewModel;

public class MainViewModel : BaseViewModel
{
    private readonly BackendOptions _options;
    private readonly HttpClient _httpClient;

    public ObservableCollection<BaseWord> BaseWords { get; set; } = new ();
    public ObservableCollection<GetExtentedWords> ExtentedWords { get; set; } = new ();
    
    public ObservableCollection<TextDto> Texts { get; set; } = new ();
    
    public string NewExtentedWord { get; set; }
    public TriggerCommand<object> SaveExtentedWordCommand { get; set; }
    public TriggerCommand<object> OpenEditWordForm { get; set; }
    
    // Base Word
    public string NewBaseWord { get; set; }
    public TriggerCommand AddBaseWordCommand { get; set; }
    public TriggerCommand<object> OpenEditBaseWordForm { get; set; }
    public TriggerCommand EditBaseWordCommand { get; set; }
    public TriggerCommand<object> DeleteBaseWordCommand { get; set; }
    public EditBaseWordRequest EditBaseWordRequest { get; set; } = new();
    public AddWordRequest AddWordRequest { get; set; } = new();
    
    // Extented Words
    public TriggerCommand<object> ExtentedWordsCommand { get; set; }
    public TriggerCommand AddExtentedWordCommand { get; set; }
    public TriggerCommand<object> OpenEditExtentedWordForm { get; set; }
    public TriggerCommand EditExtentedWordCommand { get; set; }
    public TriggerCommand<object> DeleteExtentedWordCommand { get; set; }
    public AddExtentedWord AddExtentedWordRequest { get; set; } = new();
    public EditExtentedWordRequest EditExtentedWordRequest { get; set; } = new();
    
    // Texts
    
    public TextDto CurrentText { get; set; }
    public string AddedText { get; set; }
    public TriggerCommand<object> GetWordByTextCommand { get; set; }
    public TriggerCommand AddTextCommand { get; set; }
    public TriggerCommand<object> DeleteTextCommand { get; set; }
    public TriggerCommand<object> OpenEditTextForm { get; set; }
    public EditTextRequest EditTextRequest { get; set; } = new();
    public TriggerCommand EditTextCommand { get; set; }
    public TriggerCommand<object> ProcessTextCommand { get; set; }
    
    
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
            Texts = await GetTexts();
            CurrentText = null;
            RaisePropertyChanged(nameof(CurrentText));
            RaisePropertyChanged(nameof(BaseWords));
            RaisePropertyChanged(nameof(Texts));
        }
        catch (Exception e)
        {
            MessageBox.Show("Ошибка соединения с Сервером"); 
        }
    }

    private void InitializeCommands()
    {
        DeleteBaseWordCommand = new TriggerCommand<object>(DeleteBaseWord);
        OpenEditBaseWordForm = new TriggerCommand<object>(EditBaseWordForm);
        EditBaseWordCommand = new TriggerCommand(EditBaseWord);
        AddBaseWordCommand = new TriggerCommand(AddBaseWord);
        
        ExtentedWordsCommand = new TriggerCommand<object>(GetExtentedWords);
        AddExtentedWordCommand =  new TriggerCommand(AddExtentedWord);
        OpenEditExtentedWordForm = new TriggerCommand<object>(EditExtentedWordForm);
        EditExtentedWordCommand = new TriggerCommand(EditExtentedWord);
        DeleteExtentedWordCommand =  new TriggerCommand<object>(DeleteExtentedWord);
        
        GetWordByTextCommand = new TriggerCommand<object>(GetWordByText);
        AddTextCommand = new TriggerCommand(AddText);
        DeleteTextCommand = new TriggerCommand<object>(DeleteText);
        OpenEditTextForm = new TriggerCommand<object>(EditTextForm);
        EditTextCommand = new TriggerCommand(EditText);
        ProcessTextCommand = new TriggerCommand<object>(ProcessText);
    }
    
  
    
    
    // Получить все слова
    private async Task<ObservableCollection<BaseWord>> GetWords()
    {
        var response = await _httpClient.GetAsync(_options.Host + "/api/Admin/Dictionary");
        var responseObj = await ResponseHandler.DeserializeAsync<ObservableCollection<BaseWord>>(response);
        
        return responseObj;
    }

    private async void DeleteBaseWord(object word)
    {
        var Datacontext = ((Button)word).DataContext;
        if (Datacontext is BaseWord _baseWord)
        {
            if (CurrentText == null)
            {
                var response = await _httpClient.DeleteAsync(_options.Host + $"/api/Admin/Dictionary/{_baseWord.Id}");
                if (response.IsSuccessStatusCode)
                {
                    BaseWords.Remove(_baseWord);
                }
            }
            else
            {
                var response = await _httpClient.DeleteAsync(_options.Host + $"/api/Admin/Text/{CurrentText.Id}/Word/{_baseWord.Id}");
                if (response.IsSuccessStatusCode)
                {
                    BaseWords.Remove(_baseWord);
                    RaisePropertyChanged(nameof(Texts));
                }
            }
        }
    }
    
    private async void AddBaseWord()
    {
        var response = await _httpClient.PostAsJsonAsync(_options.Host + "/api/Admin/Dictionary", AddWordRequest);
        
        if (response.IsSuccessStatusCode)
        {
            var responseObj = await ResponseHandler.DeserializeAsync<BaseWord>(response);
            BaseWords.Add(responseObj);
        }
        else
        {
            MessageBox.Show(await response.Content.ReadAsStringAsync());
        }
        AddWordRequest = new AddWordRequest();
        RaisePropertyChanged(nameof(AddWordRequest));
    }
    
    private async void EditBaseWordForm(object word)
    {
        var Datacontext = ((Button)word).DataContext;
        if(Datacontext is BaseWord _word) // rework
        {
            EditBaseWordRequest.Id = _word.Id;
            EditBaseWordRequest.Word = _word.Word;
            EditBaseWordRequest.Translation = _word.Translation;
        }

       
        var win = new EditBaseWord(this);
        win.Show();  
    }

    private async void EditBaseWord()
    {
        var response = await _httpClient.PutAsJsonAsync(_options.Host + $"/api/Admin/Dictionary/{EditBaseWordRequest.Id}", EditBaseWordRequest);
        if (response.IsSuccessStatusCode)
        {
            var responseObj = await ResponseHandler.DeserializeAsync<BaseWord>(response);
            
            var objToEdit = BaseWords.FirstOrDefault(i => i.Id == responseObj.Id);
            
            if (objToEdit != null)
            {
                int i = BaseWords.IndexOf(objToEdit);
                BaseWords[i] = responseObj;
            }
        }
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
        else
        {
            MessageBox.Show(await response.Content.ReadAsStringAsync());
        }
        AddExtentedWordRequest = new AddExtentedWord();
        RaisePropertyChanged(nameof(AddExtentedWordRequest));
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
    
    // Получить все слова
    private async Task<ObservableCollection<TextDto>> GetTexts()
    {
        var response = await _httpClient.GetAsync(_options.Host + "/api/Admin/Text");
        var responseObj = await ResponseHandler.DeserializeAsync<ObservableCollection<TextDto>>(response);
        
        return responseObj;
    }

    // Получить слова по тексту
    private async void GetWordByText(object text)
    {
        var Datacontext = ((Button)text).DataContext;
        if (Datacontext is TextDto _text)
        {
            var response = await _httpClient.GetAsync(_options.Host + $"/api/Admin/Text/{_text.Id}");
            var responseObj = await ResponseHandler.DeserializeAsync<GetWordsByTextResponse>(response);
            BaseWords.Clear();

            foreach (var word in responseObj.Words)
            {
                var baseWord = new BaseWord()
                {
                    Id = word.Id,
                    Word = word.Word,
                    Translation = word.Translation,
                };
                BaseWords.Add(baseWord);
            }
            CurrentText = _text;
            RaisePropertyChanged(nameof(CurrentText));
        }
    }

    // Удалить текст
    private async void DeleteText(object text)
    {
        var Datacontext = ((Button)text).DataContext;
        if (Datacontext is TextDto _text)
        {
            var response = await _httpClient.DeleteAsync(_options.Host + $"/api/Admin/Text/{_text.Id}");
            if (response.IsSuccessStatusCode)
            {
                Texts.Remove(_text);
            }
        }
    }

   

    private async void AddText()
    {
        var response = await _httpClient.PostAsJsonAsync(_options.Host + "/api/Admin/Text", AddedText);
        if (response.IsSuccessStatusCode)
        {
            var responseObj = await ResponseHandler.DeserializeAsync<TextDto>(response);
            Texts.Add(responseObj);
        }
    }
    
    private async void EditTextForm(object text)
    {
        var Datacontext = ((Button)text).DataContext;
        if(Datacontext is TextDto _text) // rework
        {
            EditTextRequest.Id = _text.Id;
            EditTextRequest.Content = _text.Content;
        }

       
        var win = new EditText(this);
        win.Show();  
    }
    
    private async void EditText()
    {
        var response = await _httpClient.PutAsJsonAsync(_options.Host + $"/api/Admin/Text/{EditTextRequest.Id}", EditTextRequest);
        if (response.IsSuccessStatusCode)
        {
            var responseObj = await ResponseHandler.DeserializeAsync<TextDto>(response);
            
            var objToEdit = Texts.FirstOrDefault(i => i.Id == responseObj.Id);
            
            if (objToEdit != null)
            {
                int i = Texts.IndexOf(objToEdit);
                Texts[i] = responseObj;
            }
        }
    }
    
    private async void ProcessText(object text)
    {
        var Datacontext = ((Button)text).DataContext;
        if (Datacontext is TextDto _text)
        {
            var response = await _httpClient.PostAsync(_options.Host + $"/api/Admin/Text/Process/{_text.Id}", new StringContent(""));;
           
        }
    }
}