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
            // Добавляем тестовую конфигурацию
            config.AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true);
        });
        
        builder.ConfigureServices(services =>
        {
            // Удаляем существующий DbContext
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<MainDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Добавляем новый DbContext с тестовым connection string
            services.AddDbContext<MainDbContext>(options =>
            {
                if (_usePostgreSql && !string.IsNullOrEmpty(_connectionString))
                {
                    options.UseNpgsql(_connectionString);
                    Console.WriteLine($"🔗 Используется PostgreSQL: {_connectionString}");
                }
                else
                {
                    options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
                    Console.WriteLine("🔗 Используется InMemory база данных");
                }
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _databaseFixture.InitializeAsync();
            
        // Получаем connection string после инициализации
        if (_usePostgreSql && _databaseFixture is PostgreFixture postgreFixture)
        {
            _connectionString = postgreFixture.ConnectionString;
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