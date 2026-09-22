using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;

namespace WeatherApp.Razor;

/// <summary>
/// Expands Razor view locations so feature controllers can resolve
/// Features/{FeatureArea}/{Slice}/{View}.cshtml via return View(...).
/// </summary>
public sealed class FeatureViewLocationExpander : IViewLocationExpander
{
    public void PopulateValues(ViewLocationExpanderContext context)
    {
        if (context.ActionContext.ActionDescriptor is ControllerActionDescriptor descriptor
            && descriptor.ControllerTypeInfo.Namespace is { } ns)
        {
            context.Values["featurenamespace"] = ns;
        }
    }

    public IEnumerable<string> ExpandViewLocations(
        ViewLocationExpanderContext context,
        IEnumerable<string> viewLocations)
    {
        if (context.ActionContext.ActionDescriptor is ControllerActionDescriptor descriptor
            && descriptor.ControllerTypeInfo.Namespace is { } ns)
        {
            const string featuresMarker = ".Features.";
            var markerIndex = ns.IndexOf(featuresMarker, StringComparison.Ordinal);

            if (markerIndex >= 0)
            {
                var featurePath = ns[(markerIndex + featuresMarker.Length)..].Replace('.', '/');
                yield return $"/Features/{featurePath}/{{0}}.cshtml";
            }
        }

        foreach (var location in viewLocations)
        {
            yield return location;
        }
    }
}
