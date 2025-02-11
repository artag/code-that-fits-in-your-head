using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace Restaurant.RestApi.Tests;

public class ReservationsTests
{
    [Fact]
    public async Task PostValidReservation()
    {
        var response = await PostReservation(
            new
            {
                at = "2023-03-10 19:00",
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
    [InlineData("2023-11-24 19:00", "juliad@example.net", "Julia Domna", 5)]
    [InlineData("2024-02-13 18:15", "x@example.com", "Xenia Ng", 9)]
    [InlineData("2023-08-23 16:15", "kite@example.edu", null, 2)]
    public async Task PostValidReservationWhenDatabaseIsEmpty(
        string at, string email, string name, int quantity)
    {
        var db = new FakeDatabase();
        var sut = new ReservationsController(db);

        var dto = new ReservationDto
        {
            At = at,
            Email = email,
            Name = name,
            Quantity = quantity,
        };

        await sut.Post(dto);

        var expected = new Reservation(
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
        var response = await PostReservation(new { at, email, name, quantity });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [SuppressMessage(
        "Usage",
        "CA2234:Pass system uri objects instead of strings",
        Justification = "URL isn't passed as variable, but as literal.")]
    private static async Task<HttpResponseMessage> PostReservation(object reservation)
    {
        await using var factory = new RestaurantApiFactory();
        var client = factory.CreateClient();
        var json = JsonSerializer.Serialize(reservation);
        using var content = new StringContent(json);
        content.Headers.ContentType!.MediaType = MediaTypeNames.Application.Json;
        return await client.PostAsync("reservations", content);
    }
}
