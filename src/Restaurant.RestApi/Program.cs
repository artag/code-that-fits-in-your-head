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

        var restaurantSettings = new Settings.RestaurantSettings();
        builder.Configuration.Bind("Restaurant", restaurantSettings);
        builder.Services.AddSingleton(restaurantSettings.ToMaitreD());
        var maitreD = restaurantSettings.ToMaitreD();
        builder.Services.AddSingleton(maitreD);

        var smtpSettings = new Settings.SmtpSettings();
        builder.Configuration.Bind("Smtp", smtpSettings);
        builder.Services.AddSingleton(smtpSettings.ToPostOffice());

        builder.Services.AddSingleton(maitreD.Tables.First());

        builder.Services.AddSingleton<IReservationsRepository>(p =>
        {
            var config = p.GetRequiredService<IConfiguration>();
            var connStr = config.GetConnectionString("Restaurant");
            return new SqliteReservationsRepository(connStr!);
        });

        builder.Services.AddSingleton<IDateTimeService, DateTimeService>();

        var app = builder.Build();
        app.UseRouting();
        app.MapControllers();

        return app.RunAsync();
    }
}
