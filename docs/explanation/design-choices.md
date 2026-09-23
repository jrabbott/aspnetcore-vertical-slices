# Design choices

Why this sample makes certain product and architecture decisions. Read this when you want context, not steps.

## Goals of the sample

- Show **Vertical Slice Architecture** on a conventional **ASP.NET Core MVC + Razor** app
- Avoid ceremony frameworks that obscure the pattern (no MediatR, AutoMapper, or generic repositories)
- Stay runnable with a **real weather API** (Open-Meteo) and a small **favourites** workflow
- Protect boundaries with **architecture, unit, and integration** tests
- Keep the repo **CI- and package-management ready** (CPM, lock files, analyzers, Dependabot)

## MVC and VSA together

MVC is the presentation mechanism; VSA is how application code is folded into use-case folders. They are complementary — see [Vertical Slice Architecture](vertical-slice-architecture.md).

## No MediatR / AutoMapper / generic repositories

Handlers are ordinary classes invoked directly from controllers. Mapping stays explicit on slice response factories. Persistence for favourites is a narrow `IFavouritesStore`, not a generic repository hierarchy. The point of the sample is to make the slice pipeline obvious in code review.

## Open-Meteo instead of a fake-only weather source

Live geocoding + forecast calls make the infrastructure seam realistic. The free non-commercial API needs no key for typical local demos. Handlers still depend on `IWeatherClient`, so tests and alternate providers can substitute implementations ([how-to](../how-to/replace-the-weather-provider.md)). Integration tests use a seeded `FakeWeatherClient` (offline); Open-Meteo HTTP mapping is covered by Infrastructure unit tests with stub handlers.

## Session-backed favourites

Production uses `SessionFavouritesStore` (cookie session + in-memory distributed cache by default). That keeps demos per-browser without a database. Closing the browser or expiring the session clears the list; different browsers do not share favourites. Shared test fakes (`FakeFavouritesStore`, `FakeWeatherClient`) live in `tests/WeatherApp.TestSupport`.

## Explicit DI registration

`Program.cs` and `Hosting/*ServiceCollectionExtensions.cs` register handlers and validators explicitly. There is no assembly scanning, so the composition root stays readable.

## UK spelling

User-facing copy, documentation prose, and code identifiers use **UK spelling** consistently (e.g. **Favourite** / **Favourites**, **organisation**).

## Related

- [Razor view discovery](razor-view-discovery.md)
- [Build system and packages](../reference/build-and-packages.md)
