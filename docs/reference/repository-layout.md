# Repository layout

```text
.
├── src/
│   ├── WeatherApp/                 # MVC host + feature slices + Razor
│   │   ├── Features/               # Vertical slices (controllers, handlers, views)
│   │   ├── Favourites/             # Session-backed IFavouritesStore
│   │   ├── Hosting/                # DI registration extensions
│   │   ├── Razor/                  # FeatureViewLocationExpander
│   │   ├── Views/Shared/           # Layout and shared chrome only
│   │   ├── _ViewImports.cshtml     # Web root (covers Features/ + Views/)
│   │   ├── _ViewStart.cshtml
│   │   └── wwwroot/
│   ├── WeatherApp.Domain/          # Domain model (no ASP.NET / Infrastructure refs)
│   └── WeatherApp.Infrastructure/  # Weather client + in-memory favourites store
├── tests/
│   ├── WeatherApp.Architecture.Tests/
│   ├── WeatherApp.Integration.Tests/
│   ├── WeatherApp.Unit.Tests/                  # Feature handlers / validators
│   ├── WeatherApp.Domain.Unit.Tests/
│   └── WeatherApp.Infrastructure.Unit.Tests/
├── docs/                           # Diátaxis documentation (this tree)
├── .github/
│   ├── workflows/ci.yml
│   └── dependabot.yml
├── aspnetcore-vertical-slices.slnx
├── Directory.Build.props
├── Directory.Packages.props
├── CodeMetricsConfig.txt
├── global.json
└── README.md
```

## Feature folders

Each use case under `src/WeatherApp/Features/Weather/<Slice>/` typically owns:

- `*Controller.cs`
- `*Request.cs` / `*Response.cs`
- `*Handler.cs`
- `*RequestValidator.cs` when input is validated
- `Index.cshtml` when the slice renders HTML

## Related

- [Vertical Slice Architecture](../explanation/vertical-slice-architecture.md)
- [Razor view discovery](../explanation/razor-view-discovery.md)
- [Build system and packages](build-and-packages.md)
