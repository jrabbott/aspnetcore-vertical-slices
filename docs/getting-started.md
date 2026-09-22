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
- `/weather/favorites`

Root (`/`) redirects to `/weather/search`.

## Routes

| Use case | Method | Route |
|---|---|---|
| Search | GET | `/weather/search` |
| Forecast | GET | `/weather/forecast` |
| Favorites | GET | `/weather/favorites` |
| Add favorite | POST | `/weather/favorites/add` |
| Remove favorite | POST | `/weather/favorites/remove` |

AddFavorite and RemoveFavorite are command slices without Razor views. They demonstrate that not every vertical slice is a page.

## Supported cities

The fake weather client supports:

- London
- Paris
- Madrid
- New York
- Tokyo

Unknown cities are handled cleanly with user-facing messages. No external API key is required.

## Tests

```bash
dotnet test aspnetcore-vertical-slices.slnx
```

Test projects:

| Project | Purpose |
|---|---|
| `tests/WeatherApp.Architecture.Tests` | Project + feature boundary rules (NetArchTest) |
| `tests/WeatherApp.Integration.Tests` | HTTP routes via `WebApplicationFactory` |
| `tests/WeatherApp.Unit.Tests` | WeatherApp feature handler + FluentValidation unit tests |
| `tests/WeatherApp.Domain.Unit.Tests` | Domain model unit tests |
| `tests/WeatherApp.Infrastructure.Unit.Tests` | Weather client + favorites store unit tests |

Architecture rules include:

- Domain does not reference Infrastructure, Web, or ASP.NET Core
- Infrastructure references Domain but not Web / MVC
- Web references Domain and Infrastructure
- Controllers and handlers live under `WeatherApp.Features`
- Handlers do not depend on MVC `Controller`
- Feature slices do not take compile-time dependencies on sibling slices
  - Each slice owns its Controller / Request / Handler (and Response for query slices)
  - Request validators live in the same feature slice as their request model

More detail: [Architecture](architecture.md).
