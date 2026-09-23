# Razor view discovery and shared configuration

This explanation covers how feature-local Razor views work in this sample, and why `_ViewImports` / `_ViewStart` live at the web project root.

For the broader VSA story, see [Vertical Slice Architecture](vertical-slice-architecture.md).

## Why views live inside Features

Feature views belong to the use case. Keeping `Index.cshtml` beside the controller, request, handler, and response improves locality of change:

```text
src/WeatherApp/Features/Weather/Search/
├── SearchController.cs
├── SearchRequest.cs
├── SearchHandler.cs
├── SearchResponse.cs
└── Index.cshtml
```

Genuinely shared Razor infrastructure remains conventional:

```text
src/WeatherApp/Views/Shared/
└── _Layout.cshtml
```

Shared layout, navigation, and reusable partials stay under `Views/Shared/` because they are application-wide — not owned by a single slice.

`_ViewImports.cshtml` and `_ViewStart.cshtml` are **not** under `Views/` — they live at the web project root so feature views inherit them (see below).

## Feature-local view discovery

Feature views intentionally live outside the conventional `Views/` directory.

This sample targets **.NET 10 / ASP.NET Core 10** and uses the standard MVC extension point `IViewLocationExpander`, configured in `Program.cs` via `RazorViewEngineOptions`, and implemented by:

`src/WeatherApp/Razor/FeatureViewLocationExpander.cs`

(The expander is MVC presentation configuration, so it lives in the web project rather than `WeatherApp.Infrastructure`.)

The expander reads the controller's namespace (for example `WeatherApp.Features.Weather.Search`) and adds:

```text
/Features/Weather/Search/{0}.cshtml
```

Controllers can use normal MVC view resolution (`return View(response)`) with no hard-coded view paths.

This approach fits ASP.NET Core 10 because:

1. `IViewLocationExpander` remains the idiomatic MVC API for customizing view lookup.
2. Configuration is centralized — controllers stay unaware of physical view paths.
3. Conventional `Views/Shared` locations continue to work for layouts and shared partials.

## Shared Razor configuration (`_ViewImports` / `_ViewStart`)

**Keep `_ViewImports.cshtml` and `_ViewStart.cshtml` at the web project root (`src/WeatherApp/`). Do not move them into `Views/` alone.**

Feature views live under `Features/...`, outside the conventional `Views/` tree. Razor discovers `_ViewImports` / `_ViewStart` by walking **up** from the view's directory toward the content root. Files under `Views/` only are **not** on that walk for a view in `Features/Search/Search.cshtml`, so those feature views would miss:

- shared `@using` / Tag Helper imports
- the default `Layout = "_Layout"` from `_ViewStart`

Placing both files at the content root means every view under `Features/` and under `Views/` inherits them:

```text
src/WeatherApp/
├── _ViewImports.cshtml   ← content root (covers Features/ and Views/)
├── _ViewStart.cshtml
├── Features/...
└── Views/Shared/...
```

That gives both `Features/**/*.cshtml` and any remaining conventional views shared namespaces, MVC Tag Helpers, and the shared `_Layout`.

No per-feature copies are required. Optional deeper `_ViewImports` / `_ViewStart` files can still override or extend settings for a subtree; the root pair remains the shared baseline.
