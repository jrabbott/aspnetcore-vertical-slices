# How to add a feature slice

Goal: add a new use case under `Features/` that follows this sample’s Vertical Slice pattern (thin controller, slice-local request/handler/response, optional Razor view, explicit DI registration).

Use this when you want to extend the weather app with another capability — not when you are learning the app for the first time ([tutorial](../tutorials/run-the-weather-app.md)).

## Prerequisites

- Solution builds: `dotnet build aspnetcore-vertical-slices.slnx`
- You know whether the slice **renders a page** (like Search) or is a **command that redirects** (like AddFavourite)

## 1. Create the slice folder

Place the slice next to its peers:

```text
src/WeatherApp/Features/Weather/<SliceName>/
```

Example for a page slice named `Alerts`:

```text
src/WeatherApp/Features/Weather/Alerts/
├── AlertsController.cs
├── AlertsRequest.cs
├── AlertsHandler.cs
├── AlertsResponse.cs
├── AlertsRequestValidator.cs   # if the request needs validation
└── Index.cshtml                # only if the slice renders HTML
```

Namespace everything as `WeatherApp.Features.Weather.<SliceName>` so [feature view discovery](../explanation/razor-view-discovery.md) can map the controller to `/Features/Weather/<SliceName>/{view}.cshtml`.

## 2. Add the request and response

Keep models **slice-local**. Prefer a little duplication over sharing DTOs with sibling slices.

- Request: bindable properties for the HTTP input (query or form).
- Response: everything the view (or redirect messaging) needs.
- Prefer static factories on the response (`Ok`, `Fail`, `FromReading`, …) instead of a shared builder type.

For command slices, still return a slice-local response even if the controller only puts a message in `TempData` and redirects.

## 3. Add FluentValidation (when there is input)

Add `<Slice>RequestValidator : AbstractValidator<<Slice>Request>` beside the request. Handlers should call `IValidator<TRequest>` so validation stays on the use-case path.

## 4. Implement the handler

The handler owns application logic for this use case:

- Validate the request
- Call infrastructure abstractions (`IWeatherClient`, `IFavouritesStore`, …) and domain types
- Return the slice response

Do **not** take a dependency on `Controller` or on types from another feature namespace. Architecture tests enforce that.

## 5. Implement the controller

Keep the controller thin:

```csharp
[Route("weather/alerts")]
public sealed class AlertsController(AlertsHandler handler) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] AlertsRequest request, CancellationToken cancellationToken)
    {
        AlertsResponse response = await handler.HandleAsync(request, cancellationToken);
        return View(response);
    }
}
```

For POST commands, validate the antiforgery token, call the handler, stash status in `TempData`, and `RedirectToAction` to a page slice.

## 6. Add a Razor view (page slices only)

Create `Index.cshtml` in the same folder with `@model ...AlertsResponse`. Layout and Tag Helpers come from the web-root `_ViewStart` / `_ViewImports` — do not move those under `Views/` alone (see [Razor view discovery](../explanation/razor-view-discovery.md)).

Shared chrome stays in `Views/Shared/` (for example `_Layout.cshtml`).

## 7. Register the slice in DI

There is no assembly scanning. In `src/WeatherApp/Hosting/WeatherAppFeatureServiceCollectionExtensions.cs`:

```csharp
services.AddTransient<IValidator<AlertsRequest>, AlertsRequestValidator>();
services.AddTransient<AlertsHandler>();
```

Controllers are discovered by MVC convention; handlers and validators are explicit.

## 8. Link navigation if users should open it

Add a nav entry in `Views/Shared/_Layout.cshtml` and/or deep links from related slices via Tag Helpers (`asp-controller`, `asp-action`). Prefer routes over sharing types across slices.

## 9. Cover it with tests

| Kind | Where | What to assert |
|---|---|---|
| Unit | `tests/WeatherApp.Unit.Tests` | Handler behaviour with fakes |
| Integration | `tests/WeatherApp.Integration.Tests` | HTTP status, redirects, AngleSharp DOM |
| Architecture | already in `FeatureBoundaryTests` | Update slice name lists if the tests enumerate features |

## 10. Verify

```bash
dotnet test aspnetcore-vertical-slices.slnx
```

## Related

- [Why VSA is organised this way](../explanation/vertical-slice-architecture.md)
- [Routes reference](../reference/routes.md)
- [How to replace the weather provider](replace-the-weather-provider.md)
