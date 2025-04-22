namespace Restaurant.RestApi;

public record DayDto
{
    public string? Date { get; set; }
    public int MaximumPartySize { get; set; }
}
