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
using AdminClient.Common;
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
    private readonly Methods _methods;

    public ObservableCollection<WordDto> BaseWords { get; set; } = new ();
    public ObservableCollection<WordDto> ExtentedWords { get; set; } = new();
    public ObservableCollection<WordPropertiesDto> WordProperties { get; set; } = new();
    public ObservableCollection<TextDto> Texts { get; set; } = new ();
    
    
    
    // Base Word
    public WordDto CurrentWord { get; set; } = new WordDto();
    public TriggerCommand AddWordCommand { get; set; }
    public TriggerCommand<object> OpenEditWordForm { get; set; }
    public TriggerCommand EditWordCommand { get; set; }
    public TriggerCommand<object> DeleteWordCommand { get; set; }
    public EditWordRequest EditWordRequest { get; set; } = new();
    public AddWordProperty AddWordRequest { get; set; } = new();
    
    // Extented Words
    public TriggerCommand<object> SelectWordCommand { get; set; }
    public AddExtentedWord AddExtentedWordRequest { get; set; } = new();
    
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
    public TriggerCommand AddWordPropertyCommand { get; set; }
    public TriggerCommand<object> DeleteWordPropertyCommand { get; set; }
    public TriggerCommand<object> OpenWordPropertyForm { get; set; }
    public TriggerCommand EditWordPropertyCommand { get; set; }
    public EditPropertyWordRequest EditPropertyWordRequest { get; set; } = new();
    public bool HealthCheck { get; set; }
    
    public MainViewModel(IOptions<BackendOptions> options,IHttpClientFactory clientFactory, Methods methods) 
    {
        _methods = methods;
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
            BaseWords =  await GetWords();
            Texts = await GetTexts();
            CurrentText = null;
            CurrentWord = null;
            HealthCheck = true;
    
            RaisePropertyChanged(nameof(CurrentWord));
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
        DeleteWordCommand = new TriggerCommand<object>(DeleteWord);
        OpenEditWordForm = new TriggerCommand<object>(EditBaseWordForm);
        EditWordCommand = new TriggerCommand(EditBaseWord);
        AddWordCommand = new TriggerCommand(AddBaseWord);
        
        SelectWordCommand = new TriggerCommand<object>(SelectWord);
        
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
    
    #region Create
    private async void AddWordProperty()
    {
        AddWordRequest.WordId = CurrentWord.Id;
        AddWordRequest.WordText = CurrentWord.WordText;
        await _methods.PostItemAsync(
            "/api/Admin/WordProperty",
            request: AddWordRequest,
            collection: WordProperties,
            onFinally: () => RaisePropertyChanged(nameof(WordProperties))
        );
    }
    
    
    private async void AddBaseWord()
    {
        await _methods.PostItemAsync(
            "/api/Admin/Dictionary",
            request: AddWordRequest,
            collection: BaseWords,
            onFinally: () =>
            {
                AddWordRequest = new AddWordProperty();
                RaisePropertyChanged(nameof(AddWordRequest));
            },
            onError: async resp =>
            {
                var err = await resp.Content.ReadAsStringAsync();
                MessageBox.Show(err);
            });
    }
    
    
    private async void AddText()
    {
        await _methods.PostItemAsync(
            "/api/Admin/Text",
            request: AddedText,
            collection: Texts,
            onError: async resp =>
            {
                var err = await resp.Content.ReadAsStringAsync();
                MessageBox.Show(err);
            });
    }

    #endregion

    #region Read
    
    // Выделить слово
    private async void SelectWord(object word)
    {
        var Datacontext = ((Button)word).DataContext;
        if (Datacontext is WordDto _word)
        {
            var response = await _httpClient.GetAsync(_options.Host + $"/api/Admin/Dictionary/{_word.Id}");
            var currentWord = await ResponseHandler.DeserializeAsync<WordDto>(response);
            CurrentWord = currentWord;
  
            WordProperties.Clear();

            ExtentedWords.Clear();
            foreach (var item in currentWord.ChildrenWords)
            {
                ExtentedWords.Add(item);
            }

            foreach (var property in CurrentWord.Properties)
            {
                WordProperties.Add(property);
            }
            
            RaisePropertyChanged(nameof(CurrentWord));
            AddExtentedWordRequest.BaseIdWord = currentWord.Id;
        }
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
                var baseWord = new WordDto()
                {
                    Id = word.Id,
                    WordText = word.WordText,
                    Translation = word.Translation,
                };
                BaseWords.Add(baseWord);
            }
            CurrentText = _text;
            RaisePropertyChanged(nameof(CurrentText));
        }
    }
    private async Task<ObservableCollection<WordDto>> GetWords()
    {
        var response = await _httpClient.GetAsync(_options.Host + "/api/Admin/Dictionary");
        var responseObj = await ResponseHandler.DeserializeAsync<ObservableCollection<WordDto>>(response);
        
        return responseObj;
    }
    
    private async Task<ObservableCollection<TextDto>> GetTexts()
    {
        var response = await _httpClient.GetAsync(_options.Host + "/api/Admin/Text");
        var responseObj = await ResponseHandler.DeserializeAsync<ObservableCollection<TextDto>>(response);
        
        return responseObj;
    }

    #endregion

    #region Update
    private async void EditWordProperty()
    {
        await _methods.UpdateAsync(
            $"/api/Admin/WordProperty/",
            request: EditPropertyWordRequest,
            collection: WordProperties,
            getId: x => x.Id);
    }
    private async void EditBaseWord()
    {
        WordDto word = new WordDto();
        if (!string.IsNullOrEmpty(EditWordRequest.ParentWord))
        {
            word = BaseWords.FirstOrDefault(x => x.WordText == EditWordRequest.ParentWord);
            if (word == null)
            {
                MessageBox.Show("Parent Word not found");
                return;
            }
        }
        
        EditWordRequest.ParentWordId = word.Id;

        await _methods.UpdateAsync(
            $"/api/Admin/Dictionary/{EditWordRequest.Id}",
            request: EditWordRequest,
            collection: BaseWords,
            getId: x => x.Id,
            onError: async resp=>
            {
                var err = await resp.Content.ReadAsStringAsync();
                MessageBox.Show(err);
            },
            onSuccess:  item =>
            {
                if (string.IsNullOrEmpty(EditWordRequest.ParentWord)) // Base -> Base
                {
                    if (BaseWords.FirstOrDefault(i => i.Id == item.Id) == null) // Extend -> Base
                    {
                        ExtentedWords.Remove(ExtentedWords.FirstOrDefault(i => i.Id == item.Id)!);
                        BaseWords.Add(item);
                    }
                }
                else 
                {
                    if (ExtentedWords.FirstOrDefault(i => i.Id == item.Id) == null) // Base -> Extend
                    {
                        ExtentedWords.Add(item);
                        BaseWords.Remove(item);
                    }
                    else // Extend -> Extend
                    {
                        ExtentedWords.Remove(ExtentedWords.FirstOrDefault(i => i.Id == item.Id)!);
                    }
                }
            });
    }
    
   
    
    private async void EditText()
    {
        await _methods.UpdateAsync(
            $"/api/Admin/Text/{EditTextRequest.Id}",
            request: EditTextRequest,
            collection: Texts,
            getId: x => x.Id);
    }
    

    #endregion
    
    #region Delete
    
    // Удаляем слово либо из словаря, либо отвязываем его от текста
    private async void DeleteWord(object word)
    {
        if (((Button)word).DataContext is WordDto _word)
        {
            await _methods.DeleteItemAsync<WordDto>(
                word,
                item => CurrentText == null
                    ? $"/api/Admin/Dictionary/{item.Id}"
                    : $"/api/Admin/Text/{CurrentText.Id}/Word/{item.Id}",
                item =>
                {
                    if (_word.ParentWordId == null)
                    {
                        BaseWords.Remove(item);
                        ExtentedWords.Clear(); 
                    }
                    else
                    {
                        var baseWordId = ExtentedWords?.FirstOrDefault(i => i.Id == item.Id)?.ParentWordId;
                        var baseWord = BaseWords.FirstOrDefault(i => i.Id == baseWordId);
                        baseWord?.ChildrenWords.Remove(item);                      
                        
                        ExtentedWords?.Remove(item);
                    }
                    
                    if (CurrentText != null)
                        RaisePropertyChanged(nameof(Texts));
                });
        }
    }
    
   
    
    // Удалить текст
    private async void DeleteText(object text)
    {
        await _methods.DeleteItemAsync<TextDto>(
            text,
            item => $"/api/Admin/Text/{item.Id}",
            item => Texts.Remove(item));
    }

    private async void DeleteWordProperty(object word)
    {
        await _methods.DeleteItemAsync<WordPropertiesDto>(
            word,
            item => $"/api/Admin/DeleteWordProperty/{item.Id}",
            item =>
            {
                WordProperties.Remove(item);
                CurrentWord.Properties.Remove(item);
            });
    }

    #endregion
    
    #region Forms

    private async void WordPropertyForm(object word)
    {
        var Datacontext = ((Button)word).DataContext;
        if(Datacontext is WordPropertiesDto _word) // rework
        {
            EditPropertyWordRequest.BaseWordId = CurrentWord.Id;
            EditPropertyWordRequest.PropertyWordId = _word.Id;
            EditPropertyWordRequest.Translation = _word.Translation;
        }
        
        var win = new EditWordProperty(this);
        win.Show();  
    }
    
    private async void EditBaseWordForm(object word)
    {
        var Datacontext = ((Button)word).DataContext;
        if(Datacontext is WordDto _word) // rework
        {
            EditWordRequest.Id = _word.Id;
            EditWordRequest.WordText = _word.WordText;
            EditWordRequest.Translation = _word.Translation;
            EditWordRequest.ParentWord = BaseWords.FirstOrDefault(i => i.Id == _word.ParentWordId)?.WordText!;
        }

       
        var win = new EditWord(this);
        win.Show();  
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

    #endregion
    
    
    private async void Translate(object word)
    {
        var Datacontext = ((Button)word).DataContext;
        if (Datacontext is WordDto _baseWord)
        {
            var response = await _httpClient.PostAsync(_options.Host + $"/api/Admin/Word/Translate/{_baseWord.Id}",new StringContent(""));
            if (response.IsSuccessStatusCode)
            {
                var responseObj = await ResponseHandler.DeserializeAsync<WordDto>(response);
                var objToEdit = BaseWords.FirstOrDefault(i => i.Id == responseObj.Id);
            
                if (objToEdit != null)
                {
                    int i = BaseWords.IndexOf(objToEdit);
                    BaseWords[i] = responseObj;
                }
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