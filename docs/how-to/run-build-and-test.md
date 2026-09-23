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

### Run one project

```bash
dotnet test tests/WeatherApp.Unit.Tests
dotnet test tests/WeatherApp.Domain.Unit.Tests
dotnet test tests/WeatherApp.Infrastructure.Unit.Tests
dotnet test tests/WeatherApp.Integration.Tests
dotnet test tests/WeatherApp.Architecture.Tests
```

Shared fakes live in `tests/WeatherApp.TestSupport` (class library, not a test project).

### What each project covers

See [Test projects](../reference/testing.md).

## Match CI locally

```bash
CI=true dotnet restore aspnetcore-vertical-slices.slnx --locked-mode
CI=true dotnet build aspnetcore-vertical-slices.slnx --configuration Release --no-restore
CI=true dotnet test aspnetcore-vertical-slices.slnx --configuration Release --no-build
```

Details: [Build system and packages](../reference/build-and-packages.md).
