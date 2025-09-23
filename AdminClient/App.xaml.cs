using System.Configuration;
using System.Data;
using System.Configuration;
using System.IO;
using System.Net.Mime;
using System.Windows;
using AdminClient.Common;
using AdminClient.Options;
using AdminClient.ViewModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AdminClient;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IHost _host;
    public static IConfiguration Configuration { get; private set; }
    
    public App()
    {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Укажите путь к файлу appsettings.json
            .AddJsonFile("appsettingsClient.json").Build();
        
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<MainViewModel>();
                services.AddSingleton<MainWindow>();
                services.AddTransient<Methods>();
                
                services.Configure<BackendOptions>(Configuration.GetSection("Backend"));
                services.AddHttpClient("HttpClient");
            })
            .Build();
    }
    
    protected override void OnStartup(StartupEventArgs e)
    {
        _host.Start();

        MainWindow = _host.Services.GetRequiredService<MainWindow>();
        MainWindow.Show();

        base.OnStartup(e);
    }
    protected override void OnExit(ExitEventArgs e)
    {
        _host.StopAsync();
        _host.Dispose();

        base.OnExit(e);
    }
}