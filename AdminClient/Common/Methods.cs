using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using AdminClient.Options;
using Microsoft.Extensions.Options;

namespace AdminClient.Common;

public class Methods
{
    
    private readonly HttpClient _httpClient;
    private readonly BackendOptions _options;

    public Methods(IOptions<BackendOptions> options, IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient("HttpClient");
        _options = options.Value;
    }
    
    public async Task PostItemAsync<TRequest, TResponse>(
        string url,
        TRequest request,
        ObservableCollection<TResponse> collection,
        Action<TResponse>? onSuccess = null,
        Func<HttpResponseMessage, Task>? onError = null,
        Action? onFinally = null)
    {
        var response = await _httpClient.PostAsJsonAsync(_options.Host + url, request);

        if (response.IsSuccessStatusCode)
        {
            var responseObj = await ResponseHandler.DeserializeAsync<TResponse>(response);
            collection.Add(responseObj);
            onSuccess?.Invoke(responseObj);
        }
        else
        {
            if (onError != null)
            {
                await onError(response);
            }
        }

        onFinally?.Invoke();
    }
    
    
    public async Task DeleteItemAsync<T>(
        object sender,
        Func<T, string> urlBuilder,
        Action<T> onSuccessRemove)
    {
        var dataContext = ((Button)sender).DataContext;
        if (dataContext is T item)
        {
            var url = urlBuilder(item);
            var response = await _httpClient.DeleteAsync(_options.Host + url);
            if (response.IsSuccessStatusCode)
            {
                onSuccessRemove(item);
            }
        }
    }
    
    public async Task UpdateAsync<TRequest, TResponse, TId>(
        string url,
        TRequest request,
        ObservableCollection<TResponse> collection,
        Func<TResponse, TId> getId,
        Func<TRequest, TId>? getRequestId = null,
        Action<TResponse>? onSuccess = null,
        Func<HttpResponseMessage, Task>? onError = null)
    {
        var response = await _httpClient.PutAsJsonAsync(_options.Host + url, request);

        if (response.IsSuccessStatusCode)
        {
            var responseObj = await ResponseHandler.DeserializeAsync<TResponse>(response);
            var id = getId(responseObj);

            var objToEdit = collection.FirstOrDefault(x => EqualityComparer<TId>.Default.Equals(getId(x), id));
            if (objToEdit != null)
            {
                int index = collection.IndexOf(objToEdit);
                collection[index] = responseObj;
            }
            onSuccess?.Invoke(responseObj);
        }
        else
        {
            if (onError != null)
                await onError(response);
        }
    }
    
    
}