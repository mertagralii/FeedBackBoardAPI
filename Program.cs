using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using FeedBackBoardAPI.Data.Contex;
using FeedBackBoardAPI.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;

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
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        
        builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
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
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAutoMapper(typeof(Program).Assembly); 
        


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();
        // Identity endpoint grubunu hem route hem Swagger tag için özelleştiriyoruz
        app.MapGroup("/user")
            .WithTags("01-user") // Swagger'da bu tag görünür
            .MyMapIdentityApi<ApplicationUser>();

        app.MapControllers();

        app.Run();
    }
}