namespace Restaurant.RestApi;

internal static class Hypertext
{
    internal static LinkDto Link(this Uri uri, string rel)
    {
        return new LinkDto { Rel = rel, Href = uri.ToString() };
    }
}
