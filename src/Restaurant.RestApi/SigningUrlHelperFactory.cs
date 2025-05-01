using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;

namespace Restaurant.RestApi;

public sealed class SigningUrlHelperFactory : IUrlHelperFactory
{
    private readonly IUrlHelperFactory _inner;
    private readonly byte[] _urlSigningKey;

    public SigningUrlHelperFactory(
        IUrlHelperFactory inner,
        byte[] urlSigningKey)
    {
        _inner = inner;
        _urlSigningKey = urlSigningKey;
    }

    public IUrlHelper GetUrlHelper(ActionContext context)
    {
        var url = _inner.GetUrlHelper(context);
        return new SigningUrlHelper(url, _urlSigningKey);
    }
}
