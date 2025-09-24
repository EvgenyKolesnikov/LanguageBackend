using System.Collections.ObjectModel;
using System.Text;
using AdminClient;
using Language.Dictionary.Requests;
using Language.Dictionary.Responses.Translate;
using Language.Model;
using Language.Services.Options;
using Language.Translate;
using Microsoft.Extensions.Options;

namespace Language.Services;

public class ExternalTranslateService
{
    
    private readonly HttpClient _httpClient;
    private readonly YandexTranslateOptions _options;
    
    public ExternalTranslateService(HttpClient client, IOptions<YandexTranslateOptions> options)
    {
        _options  = options.Value;
        _httpClient = client;
    }



   
    public async Task<MyMemoryTranslate> TranslateMyMemory(string text)
    {
        var response = await _httpClient.GetAsync($"https://api.mymemory.translated.net/get?q={text}&langpair=en|ru");
        var responseObj = await ResponseHandler.DeserializeAsync<MyMemoryTranslate>(response);
        
        return responseObj;
    }

    public async Task<YandexTranslateResponse> TranslateYandex(string text)
    {
        var body = new YandexTranslateRequest()
        {
            folderId = _options.FolderId,
            texts = text,
            targetLanguageCode = "ru",
        };
        
        var request = new HttpRequestMessage(HttpMethod.Post, "https://translate.api.cloud.yandex.net/translate/v2/translate");
        request.Headers.Add("Authorization", _options.ApiKey);
        var json = System.Text.Json.JsonSerializer.Serialize(body);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.SendAsync(request);
        var responseObj = await ResponseHandler.DeserializeAsync<YandexTranslateResponse>(response);
        
        return responseObj;
    }
}