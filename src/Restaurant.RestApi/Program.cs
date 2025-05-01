using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Restaurant.RestApi.Options;

namespace Restaurant.RestApi;

/// <summary>
/// Program.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Design",
    "CA1052:Static holder types should be Static or NotInheritable",
    Justification = "Class Program is not static for testing purposes")]
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Roslynator",
    "RCS1102:Make class static",
    Justification = "Class Program is not static for testing purposes")]
public class Program
{
    internal static Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var urlSigningKey = Encoding.ASCII.GetBytes(
            builder.Configuration.GetValue<string>("UrlSigningKey")!);

        builder.Services
            .AddControllers(opts =>
            {
                opts.Filters.Add<LinksFilter>();
                opts.Filters.Add(
                    new UrlIntegrityFilter(urlSigningKey));
            })
            .AddJsonOptions(opts =>
                opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

        ConfigureUrSigning(builder.Services, urlSigningKey);
        ConfigureAuthorization(builder);

        var restaurantOptions = new RestaurantOptions();
        builder.Configuration.Bind("Restaurant", restaurantOptions);
        builder.Services.AddSingleton(restaurantOptions.ToMaitreD());

        var smtpOptions = new Settings.SmtpOptions();
        builder.Configuration.Bind("Smtp", smtpOptions);
        builder.Services.AddSingleton(smtpOptions.ToPostOffice());

        builder.Services.AddSingleton<IReservationsRepository>(p =>
        {
            var config = p.GetRequiredService<IConfiguration>();
            var connStr = config.GetConnectionString("Restaurant");
            return new SqliteReservationsRepository(connStr!);
        });

        builder.Services.AddSingleton<IDateTimeService, DateTimeService>();

        var app = builder.Build();
        app.UseAuthentication();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();

        return app.RunAsync();
    }

    private static void ConfigureUrSigning(
        IServiceCollection services,
        byte[] urlSigningKey)
    {
        services.RemoveAll<IUrlHelperFactory>();
        services.AddSingleton<IUrlHelperFactory>(
            new SigningUrlHelperFactory(
                new UrlHelperFactory(),
                urlSigningKey));
    }

    private static void ConfigureAuthorization(WebApplicationBuilder builder)
    {
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        builder.Services.AddAuthentication(opts =>
        {
            opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(opts =>
        {
#pragma warning disable CA5404 // Do not disable token validation checks
            var secret = builder.Configuration["JwtIssuerSigningKey"];
            opts.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(secret!)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RoleClaimType = "role"
            };
            opts.RequireHttpsMetadata = false;
#pragma warning restore CA5404 // Do not disable token validation checks
        });
    }
}
