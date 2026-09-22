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
        ArgumentNullException.ThrowIfNull(context);

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
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(viewLocations);

        if (context.ActionContext.ActionDescriptor is ControllerActionDescriptor descriptor
            && descriptor.ControllerTypeInfo.Namespace is { } ns)
        {
            const string featuresMarker = ".Features.";
            int markerIndex = ns.IndexOf(featuresMarker, StringComparison.Ordinal);

            if (markerIndex >= 0)
            {
                string featurePath = ns[(markerIndex + featuresMarker.Length)..].Replace('.', '/');
                yield return $"/Features/{featurePath}/{{0}}.cshtml";
            }
        }

        foreach (string location in viewLocations)
        {
            yield return location;
        }
    }
}
