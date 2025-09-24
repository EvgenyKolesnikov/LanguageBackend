using System.Reflection;
using System.Text;
using Language.Database;
using Language.Services;
using Language.Services.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Language
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }


        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Укажите путь к файлу appsettings.json
                .AddJsonFile("appsettings.json").Build();;
            Environment = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
          
            
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            
            services.AddEndpointsApiExplorer();
            services.AddHttpClient();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Language API",
                });

                // using System.Reflection;
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
            AddJwtAuthentication(services);
            AddDbContext(services);
            
            
            services.AddTransient<DictionaryService>();
            services.AddTransient<AuthService>();
            services.AddTransient<TextService>();
            
            services.AddTransient<ExternalTranslateService>();
            services.Configure<YandexTranslateOptions>(Configuration.GetSection("YandexTranslate"));
            services.AddTransient<TranslateService>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI();
            
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors(builder =>
            {

                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowAnyOrigin();

            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }



        private void AddJwtAuthentication(IServiceCollection services)
        {
            services.Configure<JwtOptions>(Configuration.GetSection("JwtOptions"));
            
            var secretKey = Configuration.GetSection("JwtOptions:SecretKey").Value;
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = signingKey,
                    ValidateIssuerSigningKey = true
                };
            });
        }
        private void AddDbContext(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("local");
            
            services.AddDbContext<MainDbContext>(options =>
            options.UseNpgsql(connectionString)
            .LogTo(Console.WriteLine, LogLevel.Information));

        }
    }
}
