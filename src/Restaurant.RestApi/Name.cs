namespace Restaurant.RestApi;

public record Name(
    string Value
)
{
    public override string ToString() =>
        Value;
}
