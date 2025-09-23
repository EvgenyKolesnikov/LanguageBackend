using System.Net.Http;
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
}