namespace Restaurant.RestApi;

public record LinkDto
{
    public string? Rel { get; set; }
    public string? Href { get; set; }
}
