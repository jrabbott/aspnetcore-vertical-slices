# ASP.NET Core MVC with Vertical Slice Architecture

ASP.NET Core MVC + Razor for presentation, with Vertical Slice Architecture for feature organisation.

A small, runnable weather application that shows how VSA can be applied to a conventional ASP.NET Core MVC + Razor Views app — without MediatR, AutoMapper, generic repositories, or other ceremony frameworks.

## Quick start

Requirements: [.NET 10 SDK](https://dotnet.microsoft.com/download)

```bash
dotnet restore aspnetcore-vertical-slices.slnx
dotnet run --project src/WeatherApp
dotnet test aspnetcore-vertical-slices.slnx
```

Full instructions: [docs/getting-started.md](docs/getting-started.md)

## Repository layout

```text
.
├── src/
│   ├── WeatherApp/                 # MVC host + feature slices + Razor
│   ├── WeatherApp.Domain/          # Domain model
│   └── WeatherApp.Infrastructure/  # Weather client + favorites store
├── tests/
│   ├── WeatherApp.Architecture.Tests/
│   ├── WeatherApp.Integration.Tests/
│   ├── WeatherApp.Unit.Tests/                  # WeatherApp feature handlers
│   ├── WeatherApp.Domain.Unit.Tests/
│   └── WeatherApp.Infrastructure.Unit.Tests/
├── docs/
│   ├── architecture.md
│   └── getting-started.md
├── aspnetcore-vertical-slices.slnx
├── Directory.Build.props
├── global.json
└── README.md
```

## Documentation

- [Architecture](docs/architecture.md) — VSA, MVC + VSA together, Razor discovery, domain/infrastructure
- [Getting started](docs/getting-started.md) — run, routes, cities, tests
- [Docs index](docs/README.md)

## Key principle

ASP.NET Core MVC + Razor for presentation, with Vertical Slice Architecture for feature organisation.

```text
Browser → Controller → Request → Handler → Infrastructure / Domain → Response → Razor View
```
