using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace Restaurant.RestApi;

[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "This class is instantiated via Reflection.")]
internal sealed class UrlIntegrityFilter : IAsyncActionFilter
{
    private readonly byte[] _urlSigningKey;

    public UrlIntegrityFilter(byte[] urlSigningKey)
    {
        _urlSigningKey = urlSigningKey;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        if (IsGetHomeRequest(context))
        {
            await next().ConfigureAwait(false);
            return;
        }

        var strippedUrl = GetUrlWithoutSignature(context);
        if (SignatureIsValid(strippedUrl, context))
        {
            await next().ConfigureAwait(false);
            return;
        }

        context.Result = new NotFoundResult();
    }

    private static bool IsGetHomeRequest(ActionExecutingContext context)
    {
        return context.HttpContext.Request.Path == "/"
               && context.HttpContext.Request.Method == "GET";
    }

    private static string GetUrlWithoutSignature(
        ActionExecutingContext context)
    {
        var restOfQuery = QueryString.Create(
            context.HttpContext.Request.Query.Where(x => x.Key != "sig"));

        var url = context.HttpContext.Request.GetEncodedUrl();
        var ub = new UriBuilder(url);
        ub.Query = restOfQuery.ToString();
        return ub.Uri.AbsoluteUri;
    }

    private bool SignatureIsValid(
        string candidate,
        ActionExecutingContext context)
    {
        var sig = context.HttpContext.Request.Query["sig"];
        var receivedSignature = Convert.FromBase64String(sig.ToString());

        using var hmac = new HMACSHA256(_urlSigningKey);
        var computedSignature =
            hmac.ComputeHash(Encoding.ASCII.GetBytes(candidate));

        return
            computedSignature.SequenceEqual(receivedSignature);
    }
}
