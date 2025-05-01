using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
            .AddControllers(opts => opts.Filters.Add<LinksFilter>())
            .AddJsonOptions(opts =>
                opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        builder.Services.AddAuthentication(opts =>
        {
            opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(opts =>
        {
#pragma warning disable CA5404 // Do not disable token validation checks
            opts.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("Let's hope that this generates more than 128 bytes...")),
                ValidateIssuer = false,
                ValidateAudience = false,
                RoleClaimType = "role"
            };
            opts.RequireHttpsMetadata = false;
#pragma warning restore CA5404 // Do not disable token validation checks
        });

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
}
