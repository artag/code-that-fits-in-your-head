namespace Restaurant.RestApi.Tests;

public class TimeOfDayTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(25)]
    public void AttemptNegativeTimeOfDay(int hours)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new TimeOfDay(TimeSpan.FromHours(hours)));
    }
}
