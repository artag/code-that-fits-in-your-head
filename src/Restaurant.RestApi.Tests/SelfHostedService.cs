using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Restaurant.RestApi.Tests;

internal sealed class SelfHostedService : WebApplicationFactory<Program>
{
    private bool _authorizeClient;

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

    internal void AuthorizeClient()
    {
        _authorizeClient = true;
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);
        ArgumentNullException.ThrowIfNull(client);

        if (!_authorizeClient)
            return;

        var token = GenerateJwtToken();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    private static string GenerateJwtToken()
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(
            "This is not the secret used in production.");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("role", "MaitreD") }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
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
        var address = await FindAddress("urn:reservations");
        return await client.PostAsync(address, content);
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

    public async Task<HttpResponseMessage> GetCurrentYear()
    {
        var client = CreateClient();
        var yearAddress = await FindAddress("urn:year");
        return await client.GetAsync(yearAddress);
    }

    public async Task<HttpResponseMessage> GetCurrentYear(string name)
    {
        var resp = await GetRestaurant(name);
        resp.EnsureSuccessStatusCode();
        var rest = await resp.ParseJsonContent<RestaurantDto>();
        var address = rest!.Links.FindAddress("urn:year");
        return await CreateClient().GetAsync(address);
    }

    public async Task<HttpResponseMessage> GetPreviousYear()
    {
        var currentResp = await GetCurrentYear();
        currentResp.EnsureSuccessStatusCode();
        var dto = await currentResp.ParseJsonContent<CalendarDto>();
        var address = dto!.Links.FindAddress("previous");

        var client = CreateClient();
        return await client.GetAsync(address);
    }

    public async Task<HttpResponseMessage> GetNextYear()
    {
        var currentResp = await GetCurrentYear();
        currentResp.EnsureSuccessStatusCode();
        var dto = await currentResp.ParseJsonContent<CalendarDto>();
        var address = dto!.Links.FindAddress("next");

        var client = CreateClient();
        return await client.GetAsync(address);
    }

    public async Task<HttpResponseMessage> GetYear(int year)
    {
        var resp = await GetCurrentYear();
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

    public async Task<HttpResponseMessage> GetCurrentMonth()
    {
        var client = CreateClient();
        var monthAddress = await FindAddress("urn:month");
        return await client.GetAsync(monthAddress);
    }

    public async Task<HttpResponseMessage> GetPreviousMonth()
    {
        var currentResp = await GetCurrentMonth();
        currentResp.EnsureSuccessStatusCode();
        var dto = await currentResp.ParseJsonContent<CalendarDto>();
        var address = dto!.Links.FindAddress("previous");

        var client = CreateClient();
        return await client.GetAsync(address);
    }

    public async Task<HttpResponseMessage> GetNextMonth()
    {
        var currentResp = await GetCurrentMonth();
        currentResp.EnsureSuccessStatusCode();
        var dto = await currentResp.ParseJsonContent<CalendarDto>();
        var address = dto!.Links.FindAddress("next");

        var client = CreateClient();
        return await client.GetAsync(address);
    }

    public async Task<HttpResponseMessage> GetMonth(int year, int month)
    {
        var resp = await GetYear(year);
        resp.EnsureSuccessStatusCode();
        var dto = await resp.ParseJsonContent<CalendarDto>();

        var target = new DateTime(year, month, 1).ToIso8601DateString();
        var monthCalendar = dto!.Days!.Single(d => d.Date == target);
        var address = monthCalendar.Links.FindAddress("urn:month");

        var client = CreateClient();
        return await client.GetAsync(address);
    }

    public async Task<HttpResponseMessage> GetCurrentDay()
    {
        var client = CreateClient();
        var dayAddress = await FindAddress("urn:day");
        return await client.GetAsync(dayAddress);
    }

    public async Task<HttpResponseMessage> GetPreviousDay()
    {
        var currentResp = await GetCurrentDay();
        currentResp.EnsureSuccessStatusCode();
        var dto = await currentResp.ParseJsonContent<CalendarDto>();
        var address = dto!.Links.FindAddress("previous");

        var client = CreateClient();
        return await client.GetAsync(address);
    }

    public async Task<HttpResponseMessage> GetNextDay()
    {
        var currentResp = await GetCurrentDay();
        currentResp.EnsureSuccessStatusCode();
        var dto = await currentResp.ParseJsonContent<CalendarDto>();
        var address = dto!.Links.FindAddress("next");

        var client = CreateClient();
        return await client.GetAsync(address);
    }

    public async Task<HttpResponseMessage> GetDay(
        int year, int month, int day)
    {
        var resp = await GetYear(year);
        resp.EnsureSuccessStatusCode();
        var dto = await resp.ParseJsonContent<CalendarDto>();

        var target = new DateTime(year, month, day).ToIso8601DateString();
        var dayCalendar = dto!.Days!.Single(d => d.Date == target);
        var address = dayCalendar.Links.FindAddress("urn:day");

        var client = CreateClient();
        return await client.GetAsync(address);
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

    public async Task<HttpResponseMessage> GetSchedule(
        int year,
        int month,
        int day)
    {
        var resp = await GetYear(year);
        resp.EnsureSuccessStatusCode();
        var dto = await resp.ParseJsonContent<CalendarDto>();

        var target = new DateTime(year, month, day).ToIso8601DateString();
        var dayCalendar = dto!.Days!.Single(d => d.Date == target);
        var address = dayCalendar.Links.FindAddress("urn:schedule");
        return await CreateClient().GetAsync(address);
    }

    private async Task<Uri> FindAddress(string rel)
    {
        var client = CreateClient();
        var requestUri = new Uri("", UriKind.Relative);
        var homeResponse = await client.GetAsync(requestUri);
        homeResponse.EnsureSuccessStatusCode();
        var homeRepresentation = await homeResponse.ParseJsonContent<HomeDto>();
        return homeRepresentation!.Links.FindAddress(rel);
    }
}
