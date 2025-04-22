using Microsoft.AspNetCore.Mvc;
using System;
using static System.Collections.Specialized.BitVector32;

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
        var index = newController.LastIndexOf(
            controllerSuffix,
            StringComparison.OrdinalIgnoreCase);
        if (0 <= index)
            newController = newController.Remove(index);
        return new UrlBuilder(_action, newController, _values);
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

    public override bool Equals(object? obj)
    {
        return obj is UrlBuilder builder &&
               _action == builder._action &&
               _controller == builder._controller &&
               EqualityComparer<object?>.Default.Equals(_values, builder._values);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_action, _controller, _values);
    }
}
