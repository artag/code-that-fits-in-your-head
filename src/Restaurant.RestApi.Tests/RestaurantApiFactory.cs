using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Restaurant.RestApi.Tests;

internal sealed class RestaurantApiFactory : WebApplicationFactory<Program>
{
    private readonly static JsonSerializerOptions _jsonSerializerOptions =
        new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IReservationsRepository>();
            services.TryAddSingleton<IReservationsRepository>(new FakeDatabase());
            services.RemoveAll<IDateTimeService>();
            services.TryAddSingleton<IDateTimeService>(_ =>
            {
                var now = DateTime.Now;
                var date = new DateTime(now.Year, now.Month, now.Day, 19, 20, 00);
                return new SpyDateTimeService(date);
            });
        });
    }

    [SuppressMessage(
        "Usage",
        "CA2234:Pass system uri objects instead of strings",
        Justification = "URL isn't passed as variable, but as literal.")]
    public async Task<HttpResponseMessage> PostReservation(object reservation)
    {
        var client = CreateClient();
        var json = JsonSerializer.Serialize(reservation);
        using var content = new StringContent(json);
        content.Headers.ContentType!.MediaType = MediaTypeNames.Application.Json;
        return await client.PostAsync("reservations", content);
    }

    public async Task<HttpResponseMessage> PutReservation(
        Uri address, object reservation)
    {
        var client = CreateClient();
        var json = JsonSerializer.Serialize(reservation);
        using var content = new StringContent(json);
        content.Headers.ContentType!.MediaType = MediaTypeNames.Application.Json;
        return await client.PutAsync(address, content);
    }

    public async Task<HttpResponseMessage> GetCurrentYear()
    {
        var client = CreateClient();

        var homeResponse =
            await client.GetAsync(new Uri("", UriKind.Relative));
        homeResponse.EnsureSuccessStatusCode();
        var homeRepresentation = await ParseHomeContent(homeResponse);
        var yearAddress =
            homeRepresentation!.Links!.Single(l => l.Rel == "urn:year").Href;
        if (yearAddress is null)
            throw new InvalidOperationException(
                "Address for current year not found.");

        return await client.GetAsync(new Uri(yearAddress));
    }

    private static async Task<HomeDto?> ParseHomeContent(
        HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<HomeDto>(
            json, _jsonSerializerOptions);
    }
}
