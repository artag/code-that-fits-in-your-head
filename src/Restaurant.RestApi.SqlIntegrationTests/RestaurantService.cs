using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Mime;
using System.Text.Json;

namespace Restaurant.RestApi.SqlIntegrationTests;

public class RestaurantService : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IReservationsRepository>();
            services.AddSingleton<IReservationsRepository>(
                new SqliteReservationsRepository(
                    ConnectionStrings.Reservations));
        });
    }

    public async Task<HttpResponseMessage> PostReservation(object reservation)
    {
        var client = CreateClient();
        var json = JsonSerializer.Serialize(reservation);
        using var content = new StringContent(json);
        content.Headers.ContentType!.MediaType = MediaTypeNames.Application.Json;
        var address = await FindAddress("urn:reservations");
        return await client.PostAsync(address, content);
    }

    private async Task<Uri> FindAddress(string rel)
    {
        var homeResponse =
            await CreateClient().GetAsync(new Uri("", UriKind.Relative));
        homeResponse.EnsureSuccessStatusCode();
        var homeRepresentation =
            await homeResponse.ParseJsonContent<HomeDto>();

        return homeRepresentation!.Links.FindAddress(rel);
    }

    public async Task<(Uri, ReservationDto)> PostReservation(
        DateTime date,
        int quantity)
    {
        var resp = await PostReservation(new ReservationDtoBuilder()
            .WithDate(date)
            .WithQuantity(quantity)
            .Build());
        resp.EnsureSuccessStatusCode();
        var dto = await resp.ParseJsonContent<ReservationDto>();

        return (resp.Headers.Location!, dto!);
    }

    public async Task<HttpResponseMessage> PutReservation(
        Uri address,
        object reservation)
    {
        var client = CreateClient();
        var json = JsonSerializer.Serialize(reservation);
        using var content = new StringContent(json);
        content.Headers.ContentType!.MediaType = MediaTypeNames.Application.Json;
        return await client.PutAsync(address, content);
    }
}
