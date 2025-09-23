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
        if (!await _context.BaseWords.AnyAsync())
        {
            var testWords = new List<BaseWord>
            {
                new BaseWord { Id = 1, Word = "take", Translation = "взять" },
                new BaseWord { Id = 2, Word = "take off", Translation = "снять" },
                new BaseWord { Id = 3, Word = "run", Translation = "бежать" },
                new BaseWord { Id = 4, Word = "swim", Translation = "плавать" },
                new BaseWord { Id = 5, Word = "fly", Translation = "летать" }
            };
            _context.BaseWords.AddRange(testWords);
            
            var formWords = new List<ExtentedWord>()
            {
                new() { Word = "took", Translation = "взял" ,BaseWordId = 1 }
            };
            _context.ExtentedWords.AddRange(formWords);
            
            await _context.SaveChangesAsync();
            
        }
    }
}