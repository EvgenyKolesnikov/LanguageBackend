global using Xunit;
using System.Net.Http.Json;
using Language.Translate;
using Language.Database;

namespace Tests;

public class TranslateTests : IClassFixture<ApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ApplicationFactory _factory;
    private readonly MainDbContext _context;
    
    public TranslateTests(ApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    
    
    [Theory]
    [ClassData(typeof(TranslateData))]
    public async Task Test1(TranslateRequest request, string expectedText, string expectedAlias, string expectedTranslation)
    {
        var content = await _client.PostAsJsonAsync("/api/Translate/Translate", request);
        var response = await content.Content.ReadFromJsonAsync<TranslateResponse>();
        
        Assert.NotNull(response);
        Assert.Equal(expectedText, response.Text);
        Assert.Equal(expectedAlias, response.Alias);
        Assert.Equal(expectedTranslation, response.Translation);
    }
}