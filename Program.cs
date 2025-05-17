using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using FeedBackBoardAPI.Data;
using FeedBackBoardAPI.Data.Contex;
using FeedBackBoardAPI.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace FeedBackBoardAPI;

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

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();

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

        builder.Services.AddEndpointsApiExplorer();

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

        builder.Services.AddAutoMapper(typeof(Program).Assembly);

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
        // wwwroot klasöründeki statik dosyaları erişilebilir yap
        app.UseStaticFiles();
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
