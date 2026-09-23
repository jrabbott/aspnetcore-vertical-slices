# Vertical Slice Architecture

This is an **explanation**: why and how this sample organises application code as vertical slices on top of ASP.NET Core MVC + Razor.

- Learning path: [Run the weather app](../tutorials/run-the-weather-app.md)
- Practical extension: [Add a feature slice](../how-to/add-a-feature-slice.md)
- Product decisions: [Design choices](design-choices.md)

## What is Vertical Slice Architecture?

Vertical Slice Architecture organises code around **user-facing use cases** rather than technical layers.

Instead of grouping files by what they are (`Controllers/`, `Services/`, `Models/`, `Views/`), you group them by what they do (`Search/`, `Forecast/`, `AddFavourite/`).

Each slice owns the code that primarily exists for that use case:

- controller (HTTP adapter)
- request / response models
- handler (use-case logic)
- Razor view (when the use case renders a page)

## Why this example uses VSA

Traditional layered organisation spreads a single change across many folders. Adding a field to Search might touch a controller, a DTO, a service, and a view in four different places.

VSA optimizes for **locality of change**: a developer can understand and modify the Search use case primarily by working inside `src/WeatherApp/Features/Weather/Search`.

Architecture tests in `tests/WeatherApp.Architecture.Tests/FeatureBoundaryTests.cs` help keep that locality honest: feature slices must not take compile-time dependencies on sibling slices (for example Search must not reference Forecast or Favourites types). Navigation between pages via routes/Tag Helpers is fine; sharing request/response/handler types across slices is not.

That remains valuable as an application grows — as long as shared concepts stay genuinely shared and slices do not become a dumping ground for unrelated logic.

## How this differs from traditional layered organisation

| Layered | Vertical slices |
|---|---|
| Controllers / Services / Models / Views | Features / Use cases |
| Organize by technical role | Organize by user capability |
| A feature change touches many folders | A feature change stays mostly in one folder |
| Shared services often accumulate unrelated methods | Handlers stay focused on one use case |

## MVC and VSA work together

MVC and VSA are **not competing architectures**.

- **MVC** describes the presentation / request-response mechanism.
- **VSA** describes how application code is organised.

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

The resulting application is still a normal ASP.NET Core MVC application using controllers, routing, model binding, Razor Views and layouts, Tag Helpers, built-in DI, and standard middleware.

VSA changes the **organisation** of the code, not the fundamental MVC programming model.

### Why controllers and Razor Views still exist

Controllers remain thin HTTP adapters. They receive the request, bind a slice-local request model, call a handler, and return a view (or redirect).

Razor Views remain the HTML rendering mechanism. Feature-specific views live next to the use case. How discovery and `_ViewImports` / `_ViewStart` work is explained in [Razor view discovery](razor-view-discovery.md).

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
- `src/WeatherApp/Favourites/SessionFavouritesStore.cs` — per-browser session store used in production

Feature handlers depend on abstractions such as `IWeatherClient`, not on concrete providers.

An in-memory `FakeFavouritesStore` and `FakeWeatherClient` live under `tests/WeatherApp.TestSupport` for tests — they are not production Infrastructure adapters.

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

For a trivial one-page app, VSA can be more structure than you need. Prefer the simplest organisation that keeps changes local. This sample uses VSA because it demonstrates how the pattern scales past a single page while remaining easy to read.

## Dependency injection

`src/WeatherApp/Program.cs` registers dependencies explicitly:

- handlers
- FluentValidation request validators
- `IWeatherClient` / `WeatherClient` via `AddOpenMeteoWeatherClient` (typed `HttpClient` → Open-Meteo; singleton geocode cache)
- `IFavouritesStore` / `SessionFavouritesStore` (ASP.NET Core session; in-memory distributed cache)
- the feature view location expander

There is no assembly scanning. Reading `Program.cs` should make the application's wiring obvious.

## Architecture tests

Architecture tests live under:

- `tests/WeatherApp.Architecture.Tests/ArchitectureTests.cs` — project boundaries
- `tests/WeatherApp.Architecture.Tests/FeatureBoundaryTests.cs` — feature slice isolation

See also [Test projects](../reference/testing.md).
