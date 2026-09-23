---
name: weatherapp-tests
description: Add or change unit, integration, and architecture tests for this ASP.NET Core VSA weather sample. Use when writing handler tests, WebApplicationFactory/AngleSharp route tests, NetArchTest rules, or fixing CI test failures.
license: MIT
metadata:
  author: weatherapp
  version: "1.0"
---

# WeatherApp tests

Write tests that match this repo’s layers and gotchas.

Read [docs/reference/testing.md](../../../docs/reference/testing.md) and [docs/how-to/run-build-and-test.md](../../../docs/how-to/run-build-and-test.md) for project tables and CI-like commands. For HTTP DOM asserts, load [references/integration-anglesharp.md](references/integration-anglesharp.md).

## Which project

| Change | Put tests in |
|---|---|
| Feature handler / FluentValidation | `tests/WeatherApp.Unit.Tests` |
| Domain types | `tests/WeatherApp.Domain.Unit.Tests` |
| Weather client / in-memory favourites store | `tests/WeatherApp.Infrastructure.Unit.Tests` |
| HTTP routes, redirects, HTML | `tests/WeatherApp.Integration.Tests` |
| Project or feature boundaries | `tests/WeatherApp.Architecture.Tests` |

## Patterns

- **Unit (features):** fake `IWeatherClient`; use `FavouritesStore` (in-memory) for store behaviour. Do not require a live Open-Meteo call.
- **Integration:** `WeatherAppFactory` + `WebApplicationFactory`; AngleSharp via `HtmlDocument.ParseAsync`. Use separate `HttpClient` instances when asserting session isolation.
- **Architecture:** update slice namespace lists / InlineData in `FeatureBoundaryTests` when adding slices.

## Gotchas

- Prefer ungeocodable sentinel cities such as `Zzqxnotacity999`. Names that look fake (Atlantis, Nowhere) can still resolve via Open-Meteo.
- Use UK spelling in assertions that match UI copy (Favourite, organisation).
- Respect `.editorconfig` naming (private fields with `_` prefix); CI sets `TreatWarningsAsErrors` when `CI=true`.
- Antiforgery: scrape the token from the page before POSTing forms (see integration reference).

## Finish

```bash
dotnet test aspnetcore-vertical-slices.slnx
```

Fix failures before considering the task done.
