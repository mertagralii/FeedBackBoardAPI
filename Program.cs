using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using FeedBackBoardAPI.Data;
using FeedBackBoardAPI.Data.Contex;
using FeedBackBoardAPI.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

namespace FeedBackBoardAPI;

#region Enpoint Slug Kısmı 1/2

public sealed class SlugifyParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        if (value == null) return null;
        string? str = value.ToString();
        if (string.IsNullOrEmpty(str)) return null;

        return Regex.Replace(str, "([a-z])([A-Z])", "$1-$2").ToLowerInvariant();
    }
}

#endregion


public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        #region Serilog, ILogger'in ayarlandığı Kısım 
        // Kurulan Paketler (Dosyaya yazdırmak için)
        //dotnet add package Serilog.AspNetCore
        // dotnet add package Serilog.Sinks.File
        // Serilog konfigürasyonu - Logları "Logs/log-.txt" dosyasına günlük olarak kaydeder
        // Aşağıdaki kod örneği biz loglarımızın bir dosya içerisinde yazılmasını istedik.
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File( // Dosyaya yazmasını söyledik.
                path: "Logs/log-.txt", // Yolunu söyledik. (Kendisi otomatik olarak açıyor.)
                rollingInterval: RollingInterval.Day, // rollingInterval parametresi günlük, saatlik veya başka periyotlarda yeni dosya oluşturur.
                retainedFileCountLimit: 7, // Son 7 günün log dosyası tutulur // retainedFileCountLimit  ile kaç eski dosya tutulacağı belirlenir.
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}" // outputTemplate ile log formatını özelleştirebiliriz.
            )
            .CreateLogger();
        builder.Host.UseSerilog(); // Serilog'u varsayılan logger olarak ayarla
            
        /*
         * Not : outputTemplate
         * Timestamp:yyyy-MM-dd HH:mm:ss → Tarih ve saat formatı (örn: 2025-05-17 07:35:25)
         * [Level:u3] → Log seviyesini 3 harf olarak (INF, ERR, WRN gibi) gösterir.
         * {Message:lj} → Mesaj içeriği (log mesajı) – lj biçimi, "left justified" yani sola hizalanmış, JSON benzeri bir format.
         * {NewLine} → Yeni satır karakteri ekler.
         * {Exception} → Eğer varsa, hataya ilişkin detayları yazdırır.
         */
        #endregion
        

        // Add services to the container.

        #region Veritabanı Bağlantısı ve Identity Kısmı (Hem User hem Role Kısmı)

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentityApiEndpoints<ApplicationUser>() 
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();

        #endregion
       

        #region Slug Kısmı 2/2

        builder.Services
            .AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

        #endregion
        

        builder.Services.AddEndpointsApiExplorer();

        #region Swagger Dökümantasyon Kısmı 

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "FeedBack Board API",
                Version = "v1",
                Description = "Kullanıcıların geri bildirim gönderdiği bir blog sistemi için API.",
                Contact = new OpenApiContact
                {
                    Name = "Mert Ağralı",
                    Email = "mmertagrali@gmail.com",
                    Url = new Uri("https://github.com/mertagralii")
                }
            });
            c.EnableAnnotations();
            
        });

        #endregion

        #region AutoMapper Kısmı 

            builder.Services.AddAutoMapper(typeof(Program).Assembly);

        #endregion
       

        var app = builder.Build();
       

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            await SeedRoles(services);
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        #region ImageFile İle Resimleri Dosya olarak almak için kullandığımız wwwroot eklemesi

        // wwwroot klasöründeki statik dosyaları erişilebilir yap
        app.UseStaticFiles();

        #endregion
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapGroup("/user")
            .WithTags("01-user")
            .MyMapIdentityApi<ApplicationUser>();

        app.MapControllers();

        app.Run();
    }

    public static async Task SeedRoles(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (await roleManager.RoleExistsAsync("Admin")) { return; }

        await roleManager.CreateAsync(new IdentityRole("Admin"));

        var adminUser = new ApplicationUser() { UserName = "admin", Email = "admin@gmail.com", EmailConfirmed = true };
        await userManager.CreateAsync(adminUser, "P99yG-wSd8T$");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}
