# Getting started

Requirements: [.NET 10 SDK](https://dotnet.microsoft.com/download)

## Restore, build, and run

```bash
dotnet restore aspnetcore-vertical-slices.slnx
dotnet build aspnetcore-vertical-slices.slnx
dotnet run --project src/WeatherApp
```

Then open the URL shown in the console (typically `http://localhost:5xxx`) and try:

- `/weather/search?city=London`
- `/weather/forecast?city=Paris`
- `/weather/favourites`

Root (`/`) redirects to `/weather/search`.

## Routes

| Use case | Method | Route |
|---|---|---|
| Search | GET | `/weather/search` |
| Forecast | GET | `/weather/forecast` |
| Favourites | GET | `/weather/favourites` |
| Add favourite | POST | `/weather/favourites/add` |
| Remove favourite | POST | `/weather/favourites/remove` |

AddFavourite and RemoveFavourite are command slices without Razor views. They demonstrate that not every vertical slice is a page.

## Cities and weather data

Weather data comes from the free [Open-Meteo](https://open-meteo.com/) geocoding + forecast APIs (no API key for non-commercial use). Any city Open-Meteo can geocode works. The UI suggests a few examples:

- London
- Paris
- Madrid
- New York
- Tokyo

Unknown / ungeocodable cities are handled with user-facing messages.

Favourites are stored **per browser session** (cookie session + in-memory cache). Closing the browser or waiting out the idle timeout clears them; different browsers do not share a list.

## Tests

```bash
dotnet test aspnetcore-vertical-slices.slnx
```

Test projects:

| Project | Purpose |
|---|---|
| `tests/WeatherApp.Architecture.Tests` | Project + feature boundary rules (NetArchTest) |
| `tests/WeatherApp.Integration.Tests` | HTTP routes via `WebApplicationFactory`, with AngleSharp DOM assertions |
| `tests/WeatherApp.Unit.Tests` | WeatherApp feature handler + FluentValidation unit tests |
| `tests/WeatherApp.Domain.Unit.Tests` | Domain model unit tests |
| `tests/WeatherApp.Infrastructure.Unit.Tests` | Weather client + favourites store unit tests |

Architecture rules include:

- Domain does not reference Infrastructure, Web, or ASP.NET Core
- Infrastructure references Domain but not Web / MVC
- Web references Domain and Infrastructure
- Controllers and handlers live under `WeatherApp.Features`
- Handlers do not depend on MVC `Controller`
- Feature slices do not take compile-time dependencies on sibling slices
  - Each slice owns its Controller / Request / Handler / Response (validators where the slice accepts input)

More detail: [Architecture](architecture.md) (including why `_ViewImports` / `_ViewStart` live at the web project root, not under `Views/` alone).

## Central Package Management

NuGet package versions are managed centrally in `Directory.Packages.props`. Project files reference packages without `Version` attributes. Restores use per-project `packages.lock.json` files (`RestorePackagesWithLockFile`).

## Build hardening

`Directory.Build.props` enables SDK analyzers (`latest-recommended`), code-style enforcement, deterministic builds, NuGet audit (high+), and artifacts output under `.artifacts/`. When `CI=true` (GitHub Actions), builds also set `ContinuousIntegrationBuild` and `TreatWarningsAsErrors`.

Style and analyzer policy lives in `.editorconfig` (formatting, naming, nullable gates, and severity overrides). Code metrics thresholds for CA1501/CA1502/CA1505/CA1506 are in `CodeMetricsConfig.txt` (included as an `AdditionalFiles` item).

## CI and dependency updates

GitHub Actions (`.github/workflows/ci.yml`) restores with `--locked-mode`, builds, and tests on pushes to `main` and on pull requests (`CI=true`).

Dependabot (`.github/dependabot.yml`) keeps noise low:

- **NuGet** (weekly): one grouped PR for minor/patch; majors grouped separately
- **GitHub Actions** (monthly): one grouped PR for all Actions

## Microsoft Testing Platform

Test projects use **xUnit.net v3** with the **Microsoft Testing Platform** runner:

- `global.json` sets `"test": { "runner": "Microsoft.Testing.Platform" }`
- `Directory.Build.props` sets `UseMicrosoftTestingPlatformRunner` and `OutputType=Exe` for `*.Tests` projects

```bash
dotnet test aspnetcore-vertical-slices.slnx
```
