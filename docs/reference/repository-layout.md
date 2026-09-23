# Repository layout

```text
.
├── src/
│   ├── WeatherApp/                 # MVC host + feature slices + Razor
│   │   ├── Features/               # Vertical slices (controllers, handlers, views)
│   │   ├── Favourites/             # Session-backed IFavouritesStore
│   │   ├── Hosting/                # DI registration extensions
│   │   ├── Razor/                  # FeatureViewLocationExpander
│   │   ├── Styles/                 # SCSS sources (→ wwwroot/css)
│   │   ├── Scripts/                # TypeScript sources (→ wwwroot/js)
│   │   ├── Views/Shared/           # Layout and shared chrome only
│   │   ├── Mvc/                    # Small MVC helpers (e.g. Accept negotiation)
│   │   ├── _ViewImports.cshtml     # Web root (covers Features/ + Views/)
│   │   ├── _ViewStart.cshtml
│   │   └── wwwroot/                # compiled css/{tokens,base,components}.css + js/site.js
│   ├── WeatherApp.Domain/          # Domain model (no ASP.NET / Infrastructure refs)
│   └── WeatherApp.Infrastructure/  # Weather client + IFavouritesStore
├── tests/
│   ├── WeatherApp.Architecture.Tests/
│   ├── WeatherApp.Integration.Tests/
│   ├── WeatherApp.Unit.Tests/                  # Feature handlers / validators
│   ├── WeatherApp.Domain.Unit.Tests/
│   ├── WeatherApp.Infrastructure.Unit.Tests/
│   └── WeatherApp.TestSupport/         # Shared fakes (FakeFavouritesStore, FakeWeatherClient)
├── docs/                           # Diátaxis documentation (this tree)
├── .agents/
│   └── skills/                     # Project Agent Skills (SKILL.md packages)
├── .github/
│   ├── workflows/ci.yml
│   ├── dependabot.yml
│   ├── ISSUE_TEMPLATE/             # bug + feature forms
│   └── PULL_REQUEST_TEMPLATE.md
├── aspnetcore-vertical-slices.slnx
├── Directory.Build.props
├── Directory.Packages.props
├── CodeMetricsConfig.txt
├── global.json
├── CONTRIBUTING.md
├── CODE_OF_CONDUCT.md
├── SECURITY.md
├── LICENSE
└── README.md
```

## Feature folders

Each use case under `src/WeatherApp/Features/<Area>/<Slice>/` (default area `Weather`) typically owns:

- `*Controller.cs`
- `*Request.cs` / `*Response.cs`
- `*Handler.cs`
- `*RequestValidator.cs` when input is validated
- `Index.cshtml` when the slice renders HTML

## Agent Skills

Project skills live in `.agents/skills/<skill-name>/SKILL.md` ([Agent Skills](https://agentskills.io/specification)). Current packages: `add-feature-slice`, `diataxis-docs`.

## Related

- [Vertical Slice Architecture](../explanation/vertical-slice-architecture.md)
- [Razor view discovery](../explanation/razor-view-discovery.md)
- [Theme and front-end assets](theme-and-assets.md)
- [Build system and packages](build-and-packages.md)
