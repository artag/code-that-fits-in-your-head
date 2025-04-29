using System.Globalization;

namespace Restaurant.RestApi;

public static class Iso8601
{
    public static string ToIso8601DateString(this DateTime date)
    {
        return date.ToString(
            "yyyy'-'MM'-'dd",
            CultureInfo.InvariantCulture);
    }

    public static string ToIso8601TimeString(this TimeSpan time) =>
        time.ToString("T", CultureInfo.InvariantCulture);

    public static string ToIso8601DateTimeString(this DateTime date) =>
        date.ToString("O", CultureInfo.InvariantCulture);
}
