using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Restaurant.RestApi.Tests;

public class ReservationsTests
{
    [Fact]
    public async Task PostValidReservation()
    {
        var response = await PostReservation(
            new
            {
                date = "2023-03-10 19:00",
                email = "katinka@example.com",
                name = "Katinka Ingabogovna",
                quantity = 2,
            }
        );

        Assert.True(
            response.IsSuccessStatusCode,
            $"Actual status code: {response.StatusCode}."
            );
    }

    [Fact]
    public async Task PostValidReservationWhenDatabaseIsEmpty()
    {
        var db = new FakeDatabase();
        var sut = new ReservationsController(db);

        var dto = new ReservationDto
        {
            At = "2023-11-24 19:00",
            Email = "juliad@example.net",
            Name = "Julia Domna",
            Quantity = 5
        };

        await sut.Post(dto);

        var expected = new Reservation(
            new DateTime(2023, 11, 24, 19, 0, 0),
            dto.Email,
            dto.Name,
            dto.Quantity);
        Assert.Contains(expected, db);
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
