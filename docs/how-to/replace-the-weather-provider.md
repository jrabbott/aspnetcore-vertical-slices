# How to replace the weather provider

Goal: swap Open-Meteo for another weather source without rewriting feature slices.

Feature handlers depend on `IWeatherClient` in `WeatherApp.Infrastructure`. The concrete `WeatherClient` talks to Open-Meteo geocoding + forecast HTTP APIs. Production wiring lives in `AddOpenMeteoWeatherClient` (`WeatherApp.Infrastructure`) and the web host’s infrastructure registration.

## 1. Keep the abstraction stable

`IWeatherClient` (and the domain types it returns, such as `WeatherReading`) are the contract slices rely on. Prefer implementing that interface rather than teaching Search/Forecast about a new HTTP API.

## 2. Add or replace the infrastructure client

In `src/WeatherApp.Infrastructure/Weather/`:

- Keep or evolve `IWeatherClient`
- Add a new client class (or change `WeatherClient`) that maps the external API into domain readings
- Keep ASP.NET / MVC types out of this project

Unit-test the client in `tests/WeatherApp.Infrastructure.Unit.Tests` with `HttpMessageHandler` fakes where possible.

## 3. Register the new client

Update `AddOpenMeteoWeatherClient` / `src/WeatherApp/Hosting/WeatherAppInfrastructureServiceCollectionExtensions.cs` (typed `HttpClient`, options, API keys via configuration, and so on). Feature registration should stay unchanged.

## 4. Adjust integration tests

`tests/WeatherApp.Integration.Tests/WeatherAppFactory` already replaces `IWeatherClient` with a seeded `FakeWeatherClient` from `WeatherApp.TestSupport` so route tests stay offline. Keep that seam when you change providers (update the factory seed if ExampleCities or expected HTML change).

## 5. Update docs that name Open-Meteo

Touch user-facing copy and docs that assume Open-Meteo ([README](../../README.md), [tutorial](../tutorials/run-the-weather-app.md), [design choices](../explanation/design-choices.md)) so operators know which API and credentials apply.

## Related

- [Domain and infrastructure boundaries](../explanation/vertical-slice-architecture.md#where-infrastructure-belongs)
- [Design choices](../explanation/design-choices.md)
