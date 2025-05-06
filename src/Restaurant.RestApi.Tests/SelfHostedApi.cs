using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Restaurant.RestApi.Tests;

public sealed class SelfHostedApi : WebApplicationFactory<Program>
{
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

    public async Task<HttpResponseMessage> GetRestaurant(string name)
    {
        var client = CreateClient();

        var homeResponse =
            await client.GetAsync(new Uri("", UriKind.Relative));
        homeResponse.EnsureSuccessStatusCode();
        var homeRepresentation =
            await homeResponse.ParseJsonContent<HomeDto>();
        var restaurant =
            homeRepresentation!.Restaurants!.First(r => r.Name == name);
        var address = restaurant.Links.FindAddress("urn:restaurant");

        return await client.GetAsync(address);
    }

    public async Task<HttpResponseMessage> PostReservation(
        string name,
        object reservation)
    {
        var json = JsonSerializer.Serialize(reservation);
        using var content = new StringContent(json);
        content.Headers.ContentType!.MediaType = MediaTypeNames.Application.Json;

        var resp = await GetRestaurant(name);
        resp.EnsureSuccessStatusCode();
        var rest = await resp.ParseJsonContent<RestaurantDto>();
        var address = rest!.Links.FindAddress("urn:reservations");

        return await CreateClient().PostAsync(address, content);
    }

    public async Task<HttpResponseMessage> GetCurrentYear(string name)
    {
        var resp = await GetRestaurant(name);
        resp.EnsureSuccessStatusCode();
        var rest = await resp.ParseJsonContent<RestaurantDto>();
        var address = rest!.Links.FindAddress("urn:year");
        return await CreateClient().GetAsync(address);
    }

    public async Task<HttpResponseMessage> GetYear(string name, int year)
    {
        var resp = await GetCurrentYear(name);
        resp.EnsureSuccessStatusCode();
        var dto = await resp.ParseJsonContent<CalendarDto>();
        if (dto!.Year == year)
            return resp;

        var rel = dto.Year < year ? "next" : "previous";

        var client = CreateClient();
        do
        {
            var address = dto.Links.FindAddress(rel);
            resp = await client.GetAsync(address);
            resp.EnsureSuccessStatusCode();
            dto = await resp.ParseJsonContent<CalendarDto>();
        } while (dto!.Year != year);

        return resp;
    }

    public async Task<HttpResponseMessage> GetDay(
        string name,
        int year,
        int month,
        int day)
    {
        var resp = await GetYear(name, year);
        resp.EnsureSuccessStatusCode();
        var dto = await resp.ParseJsonContent<CalendarDto>();

        var target = new DateTime(year, month, day).ToIso8601DateString();
        var dayCalendar = dto!.Days!.Single(d => d.Date == target);
        var address = dayCalendar.Links.FindAddress("urn:day");
        return await CreateClient().GetAsync(address);
    }
}
