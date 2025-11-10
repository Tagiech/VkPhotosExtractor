using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Polly;
using VkPhotosExtractor.Application;
using VkPhotosExtractor.Application.Auth;
using VkPhotosExtractor.Application.Cache;
using VkPhotosExtractor.Application.Configurations;
using VkPhotosExtractor.Cache.Auth;
using VkPhotosExtractor.Cache.Users;
using VkPhotosExtractor.Integration.VkClient;
using VkPhotosExtractor.Web.Configs;
using VkPhotosExtractor.Web.Middlewares;

namespace VkPhotosExtractor.Web;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddAppSettings(builder.Configuration);

        builder.Services.AddJwtAuthentication(builder.Configuration);
        builder.Services.AddCorsPolicy(builder.Configuration);
        builder.Services.ConfigureForwardedHeaders();
        builder.Services.AddAuthorization();
        builder.Services.AddVkIdHttpClient();
        
        builder.Services.AddMemoryCache();
        builder.Services.AddServices();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllers();

        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseForwardedHeaders();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<JwtCookieMiddleware>();
        app.UseCors("CorsPolicy");
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static void AddAppSettings(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<VkConfig>(configuration.GetSection("VkConfig"));
        services.Configure<JwtConfig>(configuration.GetSection("Jwt"));
    }
    
    private static void AddJwtAuthentication(this IServiceCollection services, ConfigurationManager configuration)
    {
        var jwtConfig = configuration.GetSection("Jwt").Get<JwtConfig>();
        if (jwtConfig?.Key is null || jwtConfig.Audience is null || jwtConfig.Issuer is null)
        {
            throw new InvalidOperationException("JWT configuration section is missing or invalid.");
        }
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key))
                };
            });
    }
    
    private static void AddCorsPolicy(this IServiceCollection services, ConfigurationManager configuration)
    {
        var allowedOrigins = configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>();
        if (allowedOrigins is null || allowedOrigins.Length == 0)
        {
            throw new InvalidOperationException( "CORS configuration section is missing or invalid.");
        }
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policy =>
            {
                policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    private static void ConfigureForwardedHeaders(this IServiceCollection services)
    {
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto |
                ForwardedHeaders.XForwardedHost;

            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });
    }
    
    private static void AddVkIdHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient("vkid", c =>
            {
                c.BaseAddress = new Uri("https://id.vk.ru/");
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddPolicyHandler(Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
    }

    private static void AddServices(this IServiceCollection services)
    {        
        services.AddSingleton<IConfigurationsProvider, ConfigurationsProvider>();

        services.AddSingleton<ISecurityStringCacheService, SecurityStringCacheService>();
        services.AddSingleton<IUserCacheService, UserCacheService>();
        
        services.AddSingleton<ISecurityStringProvider, SecurityStringProvider>();
        services.AddSingleton<IAuthService, AuthService>();

        services.AddSingleton<IVkIdClient, VkIdClient>();
    }
}