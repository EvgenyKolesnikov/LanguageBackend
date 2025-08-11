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
using Language.Dictionary.Responses.Translate;

namespace AdminClient.ViewModel;

public class MainViewModel : BaseViewModel
{
    private readonly BackendOptions _options;
    private readonly HttpClient _httpClient;

    public ObservableCollection<BaseWordDto> BaseWords { get; set; } = new ();
    public ObservableCollection<WordPropertiesDto> WordProperties { get; set; } = new();
    public ObservableCollection<GetExtentedWords> ExtentedWords { get; set; } = new ();
    
    public ObservableCollection<TextDto> Texts { get; set; } = new ();
    
    // Base Word
    public BaseWordDto CurrentBaseWord { get; set; } = new BaseWordDto();
    public TriggerCommand AddBaseWordCommand { get; set; }
    public TriggerCommand<object> OpenEditBaseWordForm { get; set; }
    public TriggerCommand EditBaseWordCommand { get; set; }
    public TriggerCommand<object> DeleteBaseWordCommand { get; set; }
    public EditBaseWordRequest EditBaseWordRequest { get; set; } = new();
    public AddWordProperty AddWordRequest { get; set; } = new();
    
    // Extented Words
    public TriggerCommand<object> SelectWordCommand { get; set; }
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
    public TriggerCommand<object> TranslateWordCommand { get; set; }
    
    // Word Property
    public AddWordProperty AddWordPropertyRequest { get; set; } = new();
    public TriggerCommand AddWordPropertyCommand { get; set; }
    public TriggerCommand<object> DeleteWordPropertyCommand { get; set; }
    public TriggerCommand<object> OpenWordPropertyForm { get; set; }
    public TriggerCommand EditWordPropertyCommand { get; set; }
    public EditPropertyWordRequest EditPropertyWordRequest { get; set; } = new();
    public bool HealthCheck { get; set; }
    
    public MainViewModel(IOptions<BackendOptions> options,IHttpClientFactory clientFactory) 
    {
        _options = options.Value;
        _httpClient = clientFactory.CreateClient("HttpClient");
        InitializeCommands();
        InitializeData();
        // Устанавливаем начальное значение для тестирования
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
            CurrentBaseWord = null;
            HealthCheck = true;
            ExtentedWords.Clear();
            RaisePropertyChanged(nameof(CurrentBaseWord));
            RaisePropertyChanged(nameof(CurrentText));
            RaisePropertyChanged(nameof(BaseWords));
            RaisePropertyChanged(nameof(Texts));
            RaisePropertyChanged(nameof(HealthCheck));
            
        }
        catch (Exception e)
        {
            HealthCheck = false;
        }
    }

    private void InitializeCommands()
    {
        DeleteBaseWordCommand = new TriggerCommand<object>(DeleteBaseWord);
        OpenEditBaseWordForm = new TriggerCommand<object>(EditBaseWordForm);
        EditBaseWordCommand = new TriggerCommand(EditBaseWord);
        AddBaseWordCommand = new TriggerCommand(AddBaseWord);
        
        SelectWordCommand = new TriggerCommand<object>(SelectBaseWord);
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

        TranslateWordCommand = new TriggerCommand<object>(Translate);
        DeleteWordPropertyCommand = new TriggerCommand<object>(DeleteWordProperty);
        OpenWordPropertyForm = new TriggerCommand<object>(WordPropertyForm);
        EditWordPropertyCommand = new TriggerCommand(EditWordProperty);
        AddWordPropertyCommand = new TriggerCommand(AddWordProperty);
    }




    private async void Translate(object word)
    {
        var Datacontext = ((Button)word).DataContext;
        if (Datacontext is BaseWordDto _baseWord)
        {
            var response = await _httpClient.PostAsync(_options.Host + $"/api/Admin/Word/Translate/{_baseWord.Id}",new StringContent(""));
            if (response.IsSuccessStatusCode)
            {
                var responseObj = await ResponseHandler.DeserializeAsync<BaseWordDto>(response);
                var objToEdit = BaseWords.FirstOrDefault(i => i.Id == responseObj.Id);
            
                if (objToEdit != null)
                {
                    int i = BaseWords.IndexOf(objToEdit);
                    BaseWords[i] = responseObj;
                }
            }
        }
    }

    private async void AddWordProperty()
    {
        AddWordRequest.BaseWordId = CurrentBaseWord.Id;
        var response = await _httpClient.PostAsJsonAsync(_options.Host + "/api/Admin/WordProperty", AddWordRequest);
        
        if (response.IsSuccessStatusCode)
        {
            var responseObj = await ResponseHandler.DeserializeAsync<WordPropertiesDto>(response);
            WordProperties.Add(responseObj);
        }
        else
        {
            MessageBox.Show(await response.Content.ReadAsStringAsync());
        }
        
        RaisePropertyChanged(nameof(WordProperties));
    }

    private async void DeleteWordProperty(object word)
    {
        var Datacontext = ((Button)word).DataContext;
        if (Datacontext is WordPropertiesDto wordProperty)
        {
            var response = await _httpClient.DeleteAsync(_options.Host + $"/api/Admin/DeleteWordProperty/{wordProperty.Id}");
            if (response.IsSuccessStatusCode)
            {
                WordProperties.Remove(wordProperty);
               
            }
        }
    }
    
    private async void WordPropertyForm(object word)
    {
        var Datacontext = ((Button)word).DataContext;
        if(Datacontext is WordPropertiesDto _word) // rework
        {
            EditPropertyWordRequest.BaseWordId = CurrentBaseWord.Id;
            EditPropertyWordRequest.PropertyWordId = _word.Id;
            EditPropertyWordRequest.Translation = _word.Translation;
        }

       
        var win = new EditWordProperty(this);
        win.Show();  
    }
    
    private async void EditWordProperty()
    {
        var response = await _httpClient.PutAsJsonAsync(_options.Host + $"/api/Admin/WordProperty/", EditPropertyWordRequest);
        if (response.IsSuccessStatusCode)
        {
            var responseObj = await ResponseHandler.DeserializeAsync<WordPropertiesDto>(response);
            
            var objToEdit = WordProperties.FirstOrDefault(i => i.Id == responseObj.Id);
            
            if (objToEdit != null)
            {
                int i = WordProperties.IndexOf(objToEdit);
                WordProperties[i] = responseObj;
            }
        }
    }
    
    
    
    // Получить все слова
    private async Task<ObservableCollection<BaseWordDto>> GetWords()
    {
        var response = await _httpClient.GetAsync(_options.Host + "/api/Admin/Dictionary");
        var responseObj = await ResponseHandler.DeserializeAsync<ObservableCollection<BaseWordDto>>(response);
        
        return responseObj;
    }

    private async void DeleteBaseWord(object word)
    {
        var Datacontext = ((Button)word).DataContext;
        if (Datacontext is BaseWordDto _baseWord)
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
            var responseObj = await ResponseHandler.DeserializeAsync<BaseWordDto>(response);
            BaseWords.Add(responseObj);
        }
        else
        {
            MessageBox.Show(await response.Content.ReadAsStringAsync());
        }
        AddWordRequest = new AddWordProperty();
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
            var responseObj = await ResponseHandler.DeserializeAsync<BaseWordDto>(response);
            
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
    
    // Выделить слово
    private async void SelectBaseWord(object word)
    {
        var Datacontext = ((Button)word).DataContext;
        if (Datacontext is BaseWordDto _baseWord)
        {
            var response = await _httpClient.GetAsync(_options.Host + $"/api/Admin/GetExtentedWord?baseWord={_baseWord.Word}");
            var responseObj = await ResponseHandler.DeserializeAsync<ObservableCollection<GetExtentedWords>>(response);
            CurrentBaseWord = _baseWord;
            ExtentedWords.Clear();
            WordProperties.Clear();

            foreach (var extentedWord in responseObj)
            {
                ExtentedWords.Add(extentedWord);
            }

            foreach (var property in CurrentBaseWord.Properties)
            {
                WordProperties.Add(property);
            }
            
            RaisePropertyChanged(nameof(CurrentBaseWord));
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
                var baseWord = new BaseWordDto()
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