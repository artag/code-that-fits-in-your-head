using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Restaurant.RestApi;

internal sealed class SigningUrlHelper : IUrlHelper
{
    private readonly IUrlHelper _inner;

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
        return _inner.Action(actionContext);
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
