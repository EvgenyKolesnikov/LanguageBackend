using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Language.Database;
using Tests.Fixtures;

namespace Tests;

public class ApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private IAsyncLifetime _databaseFixture;
    private string _connectionString;
    private bool _usePostgreSql;

    public ApplicationFactory()
    {
        _databaseFixture = new PostgreFixture();
        _usePostgreSql = true;
        ClientOptions.AllowAutoRedirect = false;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.SuppressStatusMessages(true);
        builder.UseEnvironment("test");
        
        // Настраиваем конфигурацию для тестов
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.Sources.Clear();
            config.AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true);
        });
    }

    public async Task InitializeAsync()
    {
        await _databaseFixture.InitializeAsync();
            
        // Получаем connection string после инициализации
        if (_usePostgreSql && _databaseFixture is PostgreFixture postgreFixture)
        {
            _connectionString = postgreFixture.ConnectionString;
            Environment.SetEnvironmentVariable("TEST_CONNECTION_STRING", _connectionString);
        }
    }

    public new async Task DisposeAsync()
    {
        try
        {
            if (_databaseFixture != null)
            {
                await _databaseFixture.DisposeAsync();
            }
        }
        finally
        {
            await base.DisposeAsync();
        }
    }
}