using System.Diagnostics.CodeAnalysis;

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
        builder.Services.AddControllers();

        var settings = new Settings.RestaurantSettings();
        builder.Configuration.Bind("Restaurant", settings);
        builder.Services.AddSingleton<MaitreD>(settings.ToMaitreD());
        builder.Services.AddSingleton<IPostOffice, NullPostOffice>();
        builder.Services.AddSingleton<IDateTimeService, DateTimeService>();

        builder.Services.AddSingleton<IReservationsRepository>(p =>
        {
            var config = p.GetRequiredService<IConfiguration>();
            var connStr = config.GetConnectionString("Restaurant");
            return new SqliteReservationsRepository(connStr!);
        });

        var app = builder.Build();
        app.UseRouting();
        app.MapControllers();

        return app.RunAsync();
    }
}

[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes")]
internal sealed class NullPostOffice : IPostOffice
{
    public Task EmailReservationCreated(Reservation reservation)
    {
        return Task.CompletedTask;
    }
}
