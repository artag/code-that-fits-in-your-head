using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

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
        builder.Services
            .AddControllers(opts =>
            {
                opts.Filters.Add<LinksFilter>();
                opts.Filters.Add<UrlIntegrityFilter>();
            })
            .AddJsonOptions(opts =>
                opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

        ConfigureAuthorization(builder);

        builder.Services.RemoveAll<IUrlHelperFactory>();
        builder.Services.AddSingleton<IUrlHelperFactory>(
            new SigningUrlHelperFactory(
                new UrlHelperFactory()));

        var restaurantSettings = new Settings.RestaurantSettings();
        builder.Configuration.Bind("Restaurant", restaurantSettings);
        builder.Services.AddSingleton(restaurantSettings.ToMaitreD());

        var smtpSettings = new Settings.SmtpSettings();
        builder.Configuration.Bind("Smtp", smtpSettings);
        builder.Services.AddSingleton(smtpSettings.ToPostOffice());

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
