using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;

namespace Restaurant.RestApi;

public sealed class SigningUrlHelperFactory : IUrlHelperFactory
{
    private readonly IUrlHelperFactory _inner;

    public SigningUrlHelperFactory(IUrlHelperFactory inner)
    {
        _inner = inner;
    }

    public IUrlHelper GetUrlHelper(ActionContext context)
    {
        var url = _inner.GetUrlHelper(context);
        return new SigningUrlHelper(url);
    }
}
