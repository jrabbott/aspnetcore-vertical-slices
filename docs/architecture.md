# Architecture

ASP.NET Core MVC + Razor for presentation, with Vertical Slice Architecture for feature organisation.

This sample shows how Vertical Slice Architecture (VSA) can be applied to a conventional ASP.NET Core MVC + Razor Views app — without MediatR, AutoMapper, generic repositories, or other ceremony frameworks.

## What is Vertical Slice Architecture?

Vertical Slice Architecture organizes code around **user-facing use cases** rather than technical layers.

Instead of grouping files by what they are (`Controllers/`, `Services/`, `Models/`, `Views/`), you group them by what they do (`Search/`, `Forecast/`, `AddFavourite/`).

Each slice owns the code that primarily exists for that use case:

- controller (HTTP adapter)
- request / response models
- handler (use-case logic)
- Razor view (when the use case renders a page)

## Why this example uses VSA

Traditional layered organization spreads a single change across many folders. Adding a field to Search might touch a controller, a DTO, a service, and a view in four different places.

VSA optimizes for **locality of change**: a developer can understand and modify the Search use case primarily by working inside `src/WeatherApp/Features/Weather/Search`.

Architecture tests in `tests/WeatherApp.Architecture.Tests/FeatureBoundaryTests.cs` help keep that locality honest: feature slices must not take compile-time dependencies on sibling slices (for example Search must not reference Forecast or Favourites types). Navigation between pages via routes/Tag Helpers is fine; sharing request/response/handler types across slices is not.

That remains valuable as an application grows — as long as shared concepts stay genuinely shared and slices do not become a dumping ground for unrelated logic.

## How this differs from traditional layered organization

| Layered | Vertical slices |
|---|---|
| Controllers / Services / Models / Views | Features / Use cases |
| Organize by technical role | Organize by user capability |
| A feature change touches many folders | A feature change stays mostly in one folder |
| Shared services often accumulate unrelated methods | Handlers stay focused on one use case |

## MVC and VSA work together

MVC and VSA are **not competing architectures**.

- **MVC** describes the presentation / request-response mechanism.
- **VSA** describes how application code is organized.

They combine cleanly:

```text
Browser
  ↓
Controller
  ↓
Request
  ↓
Handler
  ↓
Infrastructure / Domain
  ↓
Response
  ↓
Razor View
```

The resulting application is still a normal ASP.NET Core MVC application using:

- Controllers
- MVC routing
- Model binding
- Razor Views
- Razor layouts
- Tag Helpers
- Built-in dependency injection
- Standard ASP.NET Core middleware

VSA changes the **organization** of the code, not the fundamental MVC programming model.

### Why controllers and Razor Views still exist

Controllers remain thin HTTP adapters. They receive the request, bind a slice-local request model, call a handler, and return a view.

Razor Views remain the HTML rendering mechanism. Feature-specific views simply live next to the use case they belong to, instead of in a distant `Views/` tree.

## Why are the Razor views inside Features?

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

`_ViewImports.cshtml` and `_ViewStart.cshtml` are **not** under `Views/` — they live at the web project root so feature views inherit them. See [Shared Razor configuration](#shared-razor-configuration-_viewimports--_viewstart).

## Feature-local Razor view discovery

Feature views intentionally live outside the conventional `Views/` directory.

This sample targets **.NET 10 / ASP.NET Core 10** and uses the standard MVC extension point:

`IViewLocationExpander`

Configured in `Program.cs` via `RazorViewEngineOptions`, and implemented by:

`src/WeatherApp/Razor/FeatureViewLocationExpander.cs`

(The expander is MVC presentation configuration, so it lives in the web project rather than `WeatherApp.Infrastructure`.)

The expander reads the controller's namespace (for example `WeatherApp.Features.Weather.Search`) and adds:

```text
/Features/Weather/Search/{0}.cshtml
```

That means controllers can use normal MVC view resolution:

```csharp
return View(response);
```

No hard-coded view paths are required.

This approach is appropriate for ASP.NET Core 10 because:

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

That gives both `Features/**/*.cshtml` and any remaining conventional views:

- shared namespaces
- MVC Tag Helpers
- the shared `_Layout`

No per-feature copies are required. Optional deeper `_ViewImports` / `_ViewStart` files can still override or extend settings for a subtree; the root pair remains the shared baseline.

## How handlers work

Handlers contain use-case / application logic.

Example flow for Search:

1. `SearchController` binds `SearchRequest`
2. Controller calls `SearchHandler.HandleAsync(...)`
3. Handler uses `IWeatherClient` (infrastructure) and domain types
4. Handler returns `SearchResponse`
5. Controller returns `View(response)`

Controllers stay thin. Handlers stay focused. There is no large `WeatherService` with unrelated methods for search, forecast, favourites, add, and remove.

## Request and response models

Request and response models belong to their slice.

- `SearchRequest` / `SearchResponse` live in Search
- `ForecastRequest` / `ForecastResponse` live in Forecast
- Command slices such as AddFavourite still return a slice-local `*Response` (for example `AddFavouriteResponse`) even when they redirect instead of rendering a view

Request validation uses **FluentValidation**, with a slice-local validator next to the request model (for example `SearchRequestValidator`). Handlers invoke `IValidator<TRequest>` so validation stays in the use-case path rather than DataAnnotations attributes.

Response construction uses **static factory methods** on the response types themselves (`SearchResponse.Empty`, `FromReading`, `Invalid`, and `AddFavouriteResponse.Ok` / `Fail`). That keeps named construction paths close to the model without introducing separate Builder/Factory classes or shared response infrastructure across slices.

It is acceptable for different response models to contain similar properties. Duplication across slices is preferred over premature extraction.

## Where infrastructure belongs

Infrastructure is a separate project (`src/WeatherApp.Infrastructure`) and stays outside feature slices.

Examples in this app:

- `src/WeatherApp.Infrastructure/Weather/IWeatherClient.cs`
- `src/WeatherApp.Infrastructure/Weather/WeatherClient.cs` — Open-Meteo geocoding + forecast HTTP client
- `src/WeatherApp.Infrastructure/Favourites/IFavouritesStore.cs`
- `src/WeatherApp.Infrastructure/Favourites/FavouritesStore.cs` — in-memory list (tests / doubles)
- `src/WeatherApp/Favourites/SessionFavouritesStore.cs` — per-browser session store used in production

Feature handlers depend on abstractions such as `IWeatherClient`, not on concrete providers.

The MVC-specific `FeatureViewLocationExpander` remains in the web project (`src/WeatherApp/Razor`) because it is presentation configuration, not application infrastructure.

## Where domain code belongs

Domain concepts live in `src/WeatherApp.Domain` and remain independent of MVC and infrastructure:

- `src/WeatherApp.Domain/Weather/Location.cs`
- `src/WeatherApp.Domain/Weather/WeatherReading.cs`

Keep the domain small. Only introduce domain types when they add clarity.

## When shared code should be introduced

Extract shared code when it represents a genuinely shared concept or capability:

- domain concepts
- infrastructure abstractions
- reusable application capabilities used by multiple slices
- globally reusable UI components

Avoid extracting:

- generic Manager / Helper / Service classes created only to remove a few duplicated lines
- large services that accumulate unrelated use cases

## When VSA may be unnecessary

For a trivial one-page app, VSA can be more structure than you need. Prefer the simplest organization that keeps changes local. This sample uses VSA because it demonstrates how the pattern scales past a single page while remaining easy to read.

## Dependency injection

`src/WeatherApp/Program.cs` registers dependencies explicitly:

- handlers
- FluentValidation request validators
- `IWeatherClient` / `WeatherClient` (typed `HttpClient` → Open-Meteo)
- `IFavouritesStore` / `SessionFavouritesStore` (ASP.NET Core session; in-memory distributed cache)
- the feature view location expander

There is no assembly scanning. Reading `Program.cs` should make the application's wiring obvious.

## Architecture tests

Architecture tests live under:

- `tests/WeatherApp.Architecture.Tests/ArchitectureTests.cs` — project boundaries
- `tests/WeatherApp.Architecture.Tests/FeatureBoundaryTests.cs` — feature slice isolation
