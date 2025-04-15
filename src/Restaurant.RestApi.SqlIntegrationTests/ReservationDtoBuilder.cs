using System.Globalization;

namespace Restaurant.RestApi.SqlIntegrationTests;

internal sealed class ReservationDtoBuilder
{
    private readonly ReservationDto _dto;

    public ReservationDtoBuilder()
    {
        _dto = new ReservationDto
        {
            At = "2020-06-30 12:30",
            Email = "x@example.com",
            Name = "",
            Quantity = 1
        };
    }

    internal ReservationDtoBuilder WithDate(DateTime newDate)
    {
        _dto.At = newDate.ToString("o", CultureInfo.InvariantCulture);
        return this;
    }

    internal ReservationDtoBuilder WithQuantity(int newQuantity)
    {
        _dto.Quantity = newQuantity;
        return this;
    }

    internal ReservationDto Build()
    {
        return _dto;
    }
}
