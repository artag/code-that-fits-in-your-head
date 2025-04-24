using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Restaurant.RestApi.Tests;

internal sealed class SelfHostedService : WebApplicationFactory<Program>
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

            services.RemoveAll<CalendarFlag>();
            services.AddSingleton(new CalendarFlag(true));
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
        var yearAddress = await FindAddress("urn:year");
        return await client.GetAsync(yearAddress);
    }

    public async Task<HttpResponseMessage> GetPreviousYear()
    {
        var currentResp = await GetCurrentYear();
        currentResp.EnsureSuccessStatusCode();
        var dto = await currentResp.ParseJsonContent<CalendarDto>();
        var address = dto!.Links!.Single(l => l.Rel == "previous").Href;
        if (address is null)
            throw new InvalidOperationException(
                "Address for relationship type previous not found.");

        var client = CreateClient();
        return await client.GetAsync(new Uri(address));
    }

    public async Task<HttpResponseMessage> GetNextYear()
    {
        var currentResp = await GetCurrentYear();
        currentResp.EnsureSuccessStatusCode();
        var dto = await currentResp.ParseJsonContent<CalendarDto>();
        var address = dto!.Links!.Single(l => l.Rel == "next").Href;
        if (address is null)
            throw new InvalidOperationException(
                "Address for relationship type next not found.");

        var client = CreateClient();
        return await client.GetAsync(new Uri(address));
    }

    public async Task<HttpResponseMessage> GetYear(int year)
    {
        var resp = await GetCurrentYear();
        resp.EnsureSuccessStatusCode();
        var dto = await resp.ParseJsonContent<CalendarDto>();
        if (dto!.Year == year)
        {
            return resp;
        }
        else if (dto.Year < year)
        {
            var client = CreateClient();
            do
            {
                var address = dto.Links!.Single(l => l.Rel == "next").Href;
                if (address is null)
                    throw new InvalidOperationException(
                        "Address for relationship type next not found.");
                resp = await client.GetAsync(new Uri(address));
                resp.EnsureSuccessStatusCode();
                dto = await resp.ParseJsonContent<CalendarDto>();
            } while (dto!.Year != year);
            return resp;
        }
        else
        {
            var client = CreateClient();
            do
            {
                var address = dto.Links!.Single(l => l.Rel == "previous").Href;
                if (address is null)
                    throw new InvalidOperationException(
                        "Address for relationship type previous not found.");
                resp = await client.GetAsync(new Uri(address));
                resp.EnsureSuccessStatusCode();
                dto = await resp.ParseJsonContent<CalendarDto>();
            } while (dto!.Year != year);
            return resp;
        }
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
        var address = dto!.Links!.Single(l => l.Rel == "previous").Href;
        if (address is null)
            throw new InvalidOperationException(
                "Address for relationship type previous not found.");

        var client = CreateClient();
        return await client.GetAsync(new Uri(address));
    }

    public async Task<HttpResponseMessage> GetNextMonth()
    {
        var currentResp = await GetCurrentMonth();
        currentResp.EnsureSuccessStatusCode();
        var dto = await currentResp.ParseJsonContent<CalendarDto>();
        var address = dto!.Links!.Single(l => l.Rel == "next").Href;
        if (address is null)
            throw new InvalidOperationException(
                "Address for relationship type previous not found.");

        var client = CreateClient();
        return await client.GetAsync(new Uri(address));
    }

    public async Task<HttpResponseMessage> GetMonth(int year, int month)
    {
        var target = new DateTime(year, month, 1);

        var resp = await GetCurrentMonth();
        resp.EnsureSuccessStatusCode();
        var dto = await resp.ParseJsonContent<CalendarDto>();
        var firstDay = new DateTime(dto!.Year, dto.Month!.Value, 1);
        if (dto.Year == year && dto.Month == month)
        {
            return resp;
        }
        else if (firstDay < target)
        {
            var client = CreateClient();
            do
            {
                var address = dto.Links!.Single(l => l.Rel == "next").Href;
                if (address is null)
                    throw new InvalidOperationException(
                        "Address for relationship type next not found.");
                resp = await client.GetAsync(new Uri(address));
                resp.EnsureSuccessStatusCode();
                dto = await resp.ParseJsonContent<CalendarDto>();
                firstDay = new DateTime(dto!.Year, dto.Month!.Value, 1);
            } while (target != firstDay);
            return resp;
        }
        else
        {
            var client = CreateClient();
            do
            {
                var address = dto.Links!.Single(l => l.Rel == "previous").Href;
                if (address is null)
                    throw new InvalidOperationException(
                        "Address for relationship type previous not found.");
                resp = await client.GetAsync(new Uri(address));
                resp.EnsureSuccessStatusCode();
                dto = await resp.ParseJsonContent<CalendarDto>();
                firstDay = new DateTime(dto!.Year, dto.Month!.Value, 1);
            } while (target != firstDay);
            return resp;
        }
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
        var address = dto!.Links!.Single(l => l.Rel == "previous").Href;
        if (address is null)
            throw new InvalidOperationException(
                "Address for relationship type previous not found.");

        var client = CreateClient();
        return await client.GetAsync(new Uri(address));
    }

    public async Task<HttpResponseMessage> GetNextDay()
    {
        var currentResp = await GetCurrentDay();
        currentResp.EnsureSuccessStatusCode();
        var dto = await currentResp.ParseJsonContent<CalendarDto>();
        var address = dto!.Links!.Single(l => l.Rel == "next").Href;
        if (address is null)
            throw new InvalidOperationException(
                "Address for relationship type next not found.");

        var client = CreateClient();
        return await client.GetAsync(new Uri(address));
    }

    public async Task<HttpResponseMessage> GetDay(
        int year, int month, int day)
    {
        var resp = await GetYear(year);
        resp.EnsureSuccessStatusCode();
        var dto = await resp.ParseJsonContent<CalendarDto>();

        var target = new DateTime(year, month, day).ToIso8601DateString();
        var dayCalendar = dto!.Days!.Single(d => d.Date == target);
        var address = dayCalendar.Links!.Single(l => l.Rel == "urn:day").Href;
        if (address is null)
            throw new InvalidOperationException(
                "Address for relationship type urn:day not found.");

        var client = CreateClient();
        return await client.GetAsync(new Uri(address));
    }

    private async Task<Uri> FindAddress(string rel)
    {
        var client = CreateClient();
        var requestUri = new Uri("", UriKind.Relative);
        var homeResponse = await client.GetAsync(requestUri);
        homeResponse.EnsureSuccessStatusCode();
        var homeRepresentation = await homeResponse.ParseJsonContent<HomeDto>();
        var address = homeRepresentation?.Links?.Single(l => l.Rel == rel).Href;
        if (address is null)
            throw new InvalidOperationException(
                $"Address for relationship type {rel} not found.");

        return new Uri(address);
    }
}
