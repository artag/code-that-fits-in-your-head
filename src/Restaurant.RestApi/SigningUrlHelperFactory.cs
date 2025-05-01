using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.DataProtection;
using System.Text;

namespace Restaurant.RestApi;

public sealed class SigningUrlHelperFactory : IUrlHelperFactory
{
    private readonly IUrlHelperFactory _inner;
    public const string Secret = "The very secret secret that's checked into source contro.";

    public SigningUrlHelperFactory(IUrlHelperFactory inner)
    {
        _inner = inner;
    }

    public IUrlHelper GetUrlHelper(ActionContext context)
    {
        var url = _inner.GetUrlHelper(context);
        return new SigningUrlHelper(url, Encoding.ASCII.GetBytes(Secret));
    }
}
