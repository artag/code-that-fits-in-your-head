using Microsoft.AspNetCore.Mvc;

namespace Restaurant.RestApi;

public sealed class UrlBuilder
{
    private readonly string? _action;
    private readonly string? _controller;
    private readonly object? _values;

    public UrlBuilder()
    {
    }

    private UrlBuilder(string? action, string? controller, object? values)
    {
        _action = action;
        _controller = controller;
        _values = values;
    }

    public UrlBuilder WithAction(string newAction)
    {
        return new UrlBuilder(newAction, _controller, _values);
    }

    public UrlBuilder WithController(string newController)
    {
        ArgumentNullException.ThrowIfNull(newController);
        const string controllerSuffix = "controller";
        return new UrlBuilder(
            _action,
            newController.Remove(newController.LastIndexOf(
                controllerSuffix,
                StringComparison.OrdinalIgnoreCase)),
            _values);
    }

    public UrlBuilder WithValues(object newValues)
    {
        return new UrlBuilder(_action, _controller, newValues);
    }

    public string? BuildAbsolute(IUrlHelper url)
    {
        ArgumentNullException.ThrowIfNull(url);
        return url.Action(
            _action,
            _controller,
            _values,
            url.ActionContext.HttpContext.Request.Scheme,
            url.ActionContext.HttpContext.Request.Host.ToUriComponent());
    }
}
