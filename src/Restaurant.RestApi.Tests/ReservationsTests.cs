using System.Globalization;
using System.Net;

namespace Restaurant.RestApi.Tests;

public class ReservationsTests
{
    [Fact]
    public async Task PostValidReservation()
    {
        await using var service = new RestaurantApiFactory();
        var response = await service.PostReservation(
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
    [InlineData("2022-03-18 17:30", "shli@example.org", "Shanghai Li", 5)]
    public async Task PostValidReservationWhenDatabaseIsEmpty(
        string at, string email, string name, int quantity)
    {
        var db = new FakeDatabase();
        var sut = new ReservationsController(db, Some.MaitreD);

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
        await using var service = new RestaurantApiFactory();
        var response = await service.PostReservation(new { at, email, name, quantity });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task OverbookAttempt()
    {
        await using var service = new RestaurantApiFactory();
        await service.PostReservation(
            new
            {
                at = "2022-03-18 17:30",
                email = "mars@example.edu",
                name = "Marina Seminova",
                quantity = 6,
            });

        var response = await service.PostReservation(
            new
            {
                at = "2022-03-18 17:30",
                email = "shli@example.org",
                name = "Shanghai Li",
                quantity = 5,
            });

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task BoolTableWhenFreeSeatingIsAvailable()
    {
        await using var service = new RestaurantApiFactory();
        await service.PostReservation(
            new
            {
                at = "2023-01-02 18:15",
                email = "net@example.net",
                name = "Ned Tucker",
                quantity = 2
            });

        var response = await service.PostReservation(
            new
            {
                at = "2023-01-02 18:30",
                email = "kant@example.edu",
                name = "Katrine Troelsen",
                quantity = 4
            });

        Assert.True(
            response.IsSuccessStatusCode,
            $"Actual status code: {response.StatusCode}.");
    }
}
