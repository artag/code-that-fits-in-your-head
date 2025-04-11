namespace Restaurant.RestApi.Tests;

public class ReservationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void QuantityMustBePositive(int invalidQuantity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Reservation(
                Guid.NewGuid(),
                new DateTime(2024, 8, 19, 11, 30, 0),
                new Email("mail@example.com"),
                new Name("Marie Petrovich"),
                invalidQuantity));
    }
}
