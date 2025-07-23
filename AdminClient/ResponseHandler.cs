using System.Net.Http;
using Newtonsoft.Json;

namespace AdminClient;

public static class ResponseHandler
{
    public static async Task<T> DeserializeAsync<T>(HttpResponseMessage response)
    {
        var responseJson = await response.Content.ReadAsStringAsync();
        var responseObj = JsonConvert.DeserializeObject<T>(responseJson);

        return responseObj;
    }
}