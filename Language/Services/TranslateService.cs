using System.Collections.ObjectModel;
using AdminClient;
using Language.Dictionary.Responses.Translate;
using Language.Model;

namespace Language.Services;

public class TranslateService
{
    
    private readonly HttpClient _httpClient;
    
    public TranslateService(HttpClient client)
    {
        _httpClient = client;
    }




    public async Task<MyMemoryTranslate> Translate(string text)
    {
        var response = await _httpClient.GetAsync($"https://api.mymemory.translated.net/get?q={text}&langpair=en|ru");
        var responseObj = await ResponseHandler.DeserializeAsync<MyMemoryTranslate>(response);
        
        return responseObj;
    }
}