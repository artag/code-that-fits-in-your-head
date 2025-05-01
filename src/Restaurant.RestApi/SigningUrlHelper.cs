using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Restaurant.RestApi;

internal sealed class SigningUrlHelper : IUrlHelper
{
    private readonly IUrlHelper _inner;
    public const string Secret = "The very secret secret that's checked into source contro.";

    public SigningUrlHelper(IUrlHelper inner)
    {
        _inner = inner;
    }

    public ActionContext ActionContext
    {
        get { return _inner.ActionContext; }
    }

    public string? Action(UrlActionContext actionContext)
    {
        var url = _inner.Action(actionContext);
        var ub = new UriBuilder(url!);
        using var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(Secret));
        var sig = Convert.ToBase64String(
            hmac.ComputeHash(Encoding.ASCII.GetBytes(url!)));

        ub.Query = new QueryString(ub.Query)
            .Add("sig", sig)
            .ToString();
        return ub.ToString();
    }

    public string? Content(string? contentPath)
    {
        return _inner.Content(contentPath);
    }

    public bool IsLocalUrl(string? url)
    {
        return _inner.IsLocalUrl(url);
    }

    public string? Link(string? routeName, object? values)
    {
        return _inner.Link(routeName, values);
    }

    public string? RouteUrl(UrlRouteContext routeContext)
    {
        return _inner.RouteUrl(routeContext);
    }
}
