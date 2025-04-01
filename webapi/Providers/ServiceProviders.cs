using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using URL_ShortenerAPI.Data.Context;
using URL_ShortenerAPI.Data.Models;
using URL_ShortenerAPI.Data.Settings;
using URL_ShortenerAPI.Repositories;
using URL_ShortenerAPI.Services.AuthServices;
using URL_ShortenerAPI.Services.JwtServices;
using URL_ShortenerAPI.Services.ShortUrlServices;
using URL_ShortenerAPI.Services.UrlShortServices;

namespace URL_ShortenerAPI.Providers;

public static class ServiceProviders
{
    public static void AddMSSQLServer(this IServiceCollection services, IConfiguration configuration)
    {
        string connection = configuration.GetConnectionString("URLShortenerConnection")!;
        services.AddDbContext<URLShortenerContext>(opts =>
        {
            opts.UseSqlServer(connection);
            opts.EnableSensitiveDataLogging();
        });
    }

    public static void AddIdentityModels(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<URLShortenerContext>()
            .AddDefaultTokenProviders();
    }

    public static void AddJWTAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
        services.AddSingleton(jwtSettings);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
            options.TokenValidationParameters = (TokenValidationParameters)jwtSettings
        );

        services.AddCors(options =>
            options.AddDefaultPolicy(builder =>
                builder.WithOrigins("*")
                       .AllowAnyHeader()
                       .AllowAnyMethod()
        ));
    }

    public static void AddAccountServices(this IServiceCollection services)
    {
        services.AddScoped<UserRepository>();
        services.AddScoped<RoleRepository>();

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthService, AuthService>();
    }

    public static void AddURLShortenerServices(this IServiceCollection services)
    {
        services.AddScoped<ShortUrlRepository>();

        services.AddScoped<IShortUrlService, ShortUrlService>();
    }
}