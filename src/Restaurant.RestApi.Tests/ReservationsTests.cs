using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Text.Json;

namespace Restaurant.RestApi.Tests;

public class ReservationsTests
{
    private static readonly JsonSerializerOptions JsonSerializerOptions =
        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    [Fact]
    public async Task PostValidReservation()
    {
        var at = CreateAt("19:00");
        await using var service = new RestaurantApiFactory();
        var response = await service.PostReservation(
            new
            {
                at = at,
                email = "katinka@example.com",
                name = "Katinka Ingabogovna",
                quantity = 2,
            });

        Assert.True(
            response.IsSuccessStatusCode,
            $"Actual status code: {response.StatusCode}."
            );
    }

    [Theory]
    [InlineData("19:00", "juliad@example.net", "Julia Domna", 5)]
    [InlineData("18:15", "x@example.com", "Xenia Ng", 9)]
    [InlineData("16:55", "kite@example.edu", null, 2)]
    [InlineData("17:30", "shli@example.org", "Shanghai Li", 5)]
    public async Task PostValidReservationWhenDatabaseIsEmpty(
        string time, string email, string name, int quantity)
    {
        var at = CreateAt(time);
        var db = new FakeDatabase();
        var sut = new ReservationsController(db, Some.MaitreD);

        var dto = new ReservationDto
        {
            Id = "59D553DA-F106-42ED-864C-814F25E8753A",
            At = at,
            Email = email,
            Name = name,
            Quantity = quantity,
        };

        await sut.Post(dto);

        var expected = new Reservation(
            Guid.Parse(dto.Id),
            DateTime.Parse(dto.At, CultureInfo.InvariantCulture),
            dto.Email,
            dto.Name ?? string.Empty,
            dto.Quantity);
        Assert.Contains(expected, db);
    }

    [Theory]
    [InlineData(null, "j@example.net", "Jay Xerxes", 1)]
    [InlineData("not a date", "w@example.edu", "Wk Hd", 8)]
    [InlineData("2023-11-30 20:01", null, "Wk Hd", 19)]
    [InlineData("2022-01-02 12:10", "3@example.org", "3 Beard", 0)]
    [InlineData("2045-12-31 11:45", "git@example.com", "Gil Tan", -1)]
    public async Task PostInvalidReservation(
        string at, string email, string name, int quantity)
    {
        await using var service = new RestaurantApiFactory();
        var response = await service.PostReservation(new { at, email, name, quantity });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task OverbookAttempt()
    {
        var now = DateTime.Now.AddDays(1);
        await using var service = new RestaurantApiFactory();
        await service.PostReservation(
            new
            {
                at = now,
                email = "mars@example.edu",
                name = "Marina Seminova",
                quantity = 6,
            });

        var response = await service.PostReservation(
            new
            {
                at = now,
                email = "shli@example.org",
                name = "Shanghai Li",
                quantity = 5,
            });

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task BoolTableWhenFreeSeatingIsAvailable()
    {
        var at = CreateAt("18:00");
        await using var service = new RestaurantApiFactory();
        await service.PostReservation(
            new
            {
                at = at,
                email = "net@example.net",
                name = "Ned Tucker",
                quantity = 2
            });

        var response = await service.PostReservation(
            new
            {
                at = at,
                email = "kant@example.edu",
                name = "Katrine Troelsen",
                quantity = 4
            });

        Assert.True(
            response.IsSuccessStatusCode,
            $"Actual status code: {response.StatusCode}.");
    }

    [Theory]
    [InlineData("19:10", "adur@example.net", "Adrienne Ursa", 2)]
    [InlineData("18:55", "emol@example.gov", "Emma Olsen", 5)]
    public async Task ReadSuccessfulReservation(
        string time, string email, string name, int quantity)
    {
        var at = CreateAt(time);
        await using var service = new RestaurantApiFactory();
        var expected = new ReservationDto
        {
            At = at,
            Email = email,
            Name = name,
            Quantity = quantity
        };
        var postResp = await service.PostReservation(expected);
        var address = FindReservationAddress(postResp);

        var getResp = await service.CreateClient().GetAsync(address);

        Assert.True(
            getResp.IsSuccessStatusCode,
            $"Actual status code: {postResp.StatusCode}.");
        var actual = await ParseReservationContent(getResp);

        Assert.NotNull(actual);
        Assert.Equal(expected, actual, new ReservationDtoComparer());
        Assert.DoesNotContain(address!.ToString(), char.IsUpper);
    }

    [SuppressMessage(
        "Usage",
        "CA2234:Pass system uri objects instead of strings",
        Justification = "URL isn't passed as variable, but as literal.")]
    [Theory]
    [InlineData("3F749AB6-2E1C-4CAE-9988-7CA708DB9252")]
    [InlineData("foo")]
    public async Task GetAbsentReservation(string id)
    {
        await using var service = new RestaurantApiFactory();
        var client = service.CreateClient();

        var resp = await client.GetAsync($"/reservations/{id}");

        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }

    private static Uri? FindReservationAddress(HttpResponseMessage response)
    {
        return response.Headers.Location;
    }

    private static async Task<ReservationDto?> ParseReservationContent(
        HttpResponseMessage actual)
    {
        var json = await actual.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ReservationDto>(json, JsonSerializerOptions);
    }

    private static string CreateAt(string time)
    {
        var dateNow = DateTime.Now.Date.AddDays(1);
        var timeAt = TimeSpan.Parse(time, CultureInfo.InvariantCulture);
        return dateNow.Add(timeAt).ToString("O");
    }
}
