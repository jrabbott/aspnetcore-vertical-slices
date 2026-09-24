# How to run, build, and test

Goal: restore, build, run, and test this solution the way CI does.

## Restore

```bash
dotnet restore aspnetcore-vertical-slices.slnx
```

Package versions come from Central Package Management (`Directory.Packages.props`). Projects use `packages.lock.json`; CI restores with `--locked-mode`.

## Build

```bash
dotnet build aspnetcore-vertical-slices.slnx
```

Artifacts land under `.artifacts/` (configured in `Directory.Build.props`). When `CI=true`, builds treat warnings as errors.

`dotnet build` also compiles SCSS (`Styles/`) and TypeScript (`Scripts/`) into `wwwroot` via NuGet MSBuild packages — no Node toolchain is required. Those CSS/JS outputs are gitignored; always build (or `dotnet run`, which builds) before expecting styled pages. See [Theme and front-end assets](../reference/theme-and-assets.md).

## Run the web app

```bash
dotnet run --project src/WeatherApp
```

Open the URL printed in the console. Root `/` redirects to `/weather/search`.

For a guided first pass, see the [tutorial](../tutorials/run-the-weather-app.md).

## Test

```bash
dotnet test aspnetcore-vertical-slices.slnx
```

Test projects use **xUnit.net v3** with the **Microsoft Testing Platform** runner (`global.json` + `Directory.Build.props`).

Solution-wide `dotnet test` runs architecture, integration, and unit projects. It does **not** run browser e2e (those need a Docker image or `E2E_BASE_URL`).

### Run one project

```bash
dotnet test tests/WeatherApp.Unit.Tests
dotnet test tests/WeatherApp.Domain.Unit.Tests
dotnet test tests/WeatherApp.Infrastructure.Unit.Tests
dotnet test tests/WeatherApp.Integration.Tests
dotnet test tests/WeatherApp.Architecture.Tests
```

Shared fakes live in `tests/WeatherApp.TestSupport` (class library, not a test project).

### E2E (Playwright + Reqnroll)

Container mode (default) — build the image first, then:

```bash
dotnet publish src/WeatherApp/WeatherApp.csproj -c Release -o .publish/web
cp .dockerignore .publish/web/.dockerignore
docker build -f Dockerfile -t weatherapp:ci ./.publish/web
E2E_IMAGE=weatherapp:ci dotnet test tests/WeatherApp.E2E.Tests -c Release -p:IncludeE2E=true
```

Deployed mode (skip Testcontainers / WireMock):

```bash
E2E_BASE_URL=https://example.com dotnet test tests/WeatherApp.E2E.Tests -c Release -p:IncludeE2E=true
```

Scenarios tagged `@a11y` run axe WCAG 2.1 A/AA checks on the main pages (same project and CI/CD job as the functional e2e scenarios).

### What each project covers

See [Test projects](../reference/testing.md).

## Match CI locally

```bash
CI=true dotnet restore aspnetcore-vertical-slices.slnx --locked-mode
CI=true dotnet build aspnetcore-vertical-slices.slnx --configuration Release --no-restore
CI=true dotnet test aspnetcore-vertical-slices.slnx --configuration Release --no-build
```

Details: [Build system and packages](../reference/build-and-packages.md).
