using Language.Database;
using Language.Model;
using Testcontainers.PostgreSql;
using Microsoft.EntityFrameworkCore;

namespace Tests.Fixtures;

public class PostgreFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container;
    private string _connectionString;
    private MainDbContext _context;
    
    public PostgreFixture()
    {
        _container = new PostgreSqlBuilder()
            .WithDatabase("LanguageTest")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithPortBinding(5434, 5432) // Используем порт 5434 для тестов
            .WithImage("postgres:15-alpine") // Используем стабильную версию
            .WithCleanUp(true) // Автоматическая очистка
            .WithAutoRemove(true) // Автоматическое удаление контейнера
            .Build();
    }
    
    public string ConnectionString => _connectionString;
    
    public async Task InitializeAsync()
    {
            await _container.StartAsync();
            _connectionString = _container.GetConnectionString();
            
            // Создаем контекст для работы с БД
            var options = new DbContextOptionsBuilder<MainDbContext>()
                .UseNpgsql(_connectionString)
                .Options;
            
            _context = new MainDbContext(options);
            
            await _context.Database.EnsureCreatedAsync();
            await SeedTestData();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync().AsTask();
    }
    
    public async Task SeedTestData()
    {
        // Добавляем тестовые данные
        if (!await _context.Words.AnyAsync())
        {
            var testWords = new List<Word>
            {
                new Word { Id = 1, WordText = "take", Translation = "взять" },
                new Word { Id = 2, WordText = "take off", Translation = "снять" },
                new Word { Id = 3, WordText = "run", Translation = "бежать" },
                new Word { Id = 4, WordText = "swim", Translation = "плавать" },
                new Word { Id = 5, WordText = "fly", Translation = "летать" }
            };
            _context.Words.AddRange(testWords);
            
            
            await _context.SaveChangesAsync();
            
        }
    }
}