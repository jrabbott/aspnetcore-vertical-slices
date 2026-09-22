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

`tests/WeatherApp.Tests` includes:

- **Unit tests** for slice handlers and `WeatherClient` (hand-written fakes, no mocking framework)
- **Integration tests** using `WebApplicationFactory<Program>` for routes, feature views, layout, and favorites add/remove
- **Architecture tests** (NetArchTest + assembly-reference checks) that enforce:
  - Domain does not reference Infrastructure, Web, or ASP.NET Core
  - Infrastructure references Domain but not Web / MVC
  - Web references Domain and Infrastructure
  - Controllers and handlers live under `WeatherApp.Features`
  - Handlers do not depend on MVC `Controller`
  - Feature slices do not take compile-time dependencies on sibling slices
  - Each slice owns its Controller / Request / Handler (and Response for query slices)
  - Feature view location expander stays in the web project

More detail: [Architecture](architecture.md).
