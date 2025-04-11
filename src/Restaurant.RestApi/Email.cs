namespace Restaurant.RestApi;

public record Email(
    string Value
)
{
    public override string ToString() =>
        Value;
}
