namespace Restaurant.RestApi.Tests;

internal sealed class SpyDateTimeService : IDateTimeService
{
    public SpyDateTimeService(DateTime value)
    {
        Now = value;
    }

    public DateTime Now { get; }
}
