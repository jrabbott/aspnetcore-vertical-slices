# ASP.NET Core MVC with Vertical Slice Architecture

ASP.NET Core MVC + Razor for presentation, with Vertical Slice Architecture for feature organisation.

A small, runnable weather application that shows how VSA can be applied to a conventional ASP.NET Core MVC + Razor Views app — without MediatR, AutoMapper, generic repositories, or other ceremony frameworks. Live weather comes from the free [Open-Meteo](https://open-meteo.com/) API (no key required for non-commercial use).

## Quick start

Requirements: [.NET 10 SDK](https://dotnet.microsoft.com/download)

```bash
dotnet restore aspnetcore-vertical-slices.slnx
dotnet run --project src/WeatherApp
dotnet test aspnetcore-vertical-slices.slnx
```

First-time walkthrough: [docs/tutorials/run-the-weather-app.md](docs/tutorials/run-the-weather-app.md)

## Repository layout

```text
.
├── src/
│   ├── WeatherApp/                 # MVC host + feature slices + Razor
│   ├── WeatherApp.Domain/          # Domain model
│   └── WeatherApp.Infrastructure/  # Weather client + IFavouritesStore
├── tests/
│   ├── WeatherApp.Architecture.Tests/
│   ├── WeatherApp.Integration.Tests/
│   ├── WeatherApp.Unit.Tests/                  # WeatherApp feature handlers
│   ├── WeatherApp.Domain.Unit.Tests/
│   ├── WeatherApp.Infrastructure.Unit.Tests/
│   └── WeatherApp.TestSupport/                 # Shared test fakes
├── docs/                           # Diátaxis: tutorials / how-to / reference / explanation
├── .agents/skills/                 # Project Agent Skills
├── .github/
│   ├── workflows/ci.yml              # restore (locked) + build + test
│   └── dependabot.yml                # grouped NuGet + Actions updates
├── aspnetcore-vertical-slices.slnx
├── Directory.Build.props
├── Directory.Packages.props          # Central Package Management
├── CodeMetricsConfig.txt
├── global.json                       # .NET 10 SDK + MTP test runner
└── README.md
```

Detail: [docs/reference/repository-layout.md](docs/reference/repository-layout.md).

## Documentation ([Diátaxis](https://diataxis.fr/))

| Need | Start here |
|---|---|
| Learn | [Run the weather app](docs/tutorials/run-the-weather-app.md) |
| Do a task | [Add a feature slice](docs/how-to/add-a-feature-slice.md) · [Run, build, and test](docs/how-to/run-build-and-test.md) |
| Look up | [Routes](docs/reference/routes.md) · [Build & packages](docs/reference/build-and-packages.md) · [Tests](docs/reference/testing.md) |
| Understand | [Vertical Slice Architecture](docs/explanation/vertical-slice-architecture.md) · [Design choices](docs/explanation/design-choices.md) |

Full map: [docs/README.md](docs/README.md).

## Key principle

ASP.NET Core MVC + Razor for presentation, with Vertical Slice Architecture for feature organisation.

```text
Browser → Controller → Request → Handler → Infrastructure / Domain → Response → Razor View
```
