# Test projects

| Project | Purpose |
|---|---|
| `tests/WeatherApp.Architecture.Tests` | Project + feature boundary rules (NetArchTest) |
| `tests/WeatherApp.Integration.Tests` | HTTP routes via `WebApplicationFactory`, AngleSharp DOM assertions |
| `tests/WeatherApp.E2E.Tests` | Browser e2e (Playwright + Reqnroll + Page Object Model); Testcontainers + WireMock by default |
| `tests/WeatherApp.Unit.Tests` | WeatherApp feature handler + FluentValidation unit tests |
| `tests/WeatherApp.Domain.Unit.Tests` | Domain model unit tests |
| `tests/WeatherApp.Infrastructure.Unit.Tests` | Weather client unit tests |
| `tests/WeatherApp.TestSupport` | Shared test fakes (`FakeFavouritesStore`, `FakeWeatherClient`) |

## Architecture rules (summary)

- Domain does not reference Infrastructure, Web, or ASP.NET Core
- Infrastructure references Domain but not Web / MVC
- Web references Domain and Infrastructure
- Controllers and handlers live under `WeatherApp.Features`
- Handlers do not depend on MVC `Controller`
- Feature slices do not take compile-time dependencies on sibling slices
  - Each slice owns its Controller / Request / Handler / Response (validators where the slice accepts input)

Primary sources:

- `tests/WeatherApp.Architecture.Tests/ArchitectureTests.cs`
- `tests/WeatherApp.Architecture.Tests/FeatureBoundaryTests.cs`

## Integration notes

- `WeatherAppFactory` replaces `IWeatherClient` with a seeded `FakeWeatherClient` so HTTP/DOM tests stay offline (no live Open-Meteo).
- Open-Meteo mapping is covered by `WeatherApp.Infrastructure.Unit.Tests` with `HttpMessageHandler` stubs.
- Session cookie handling isolates favourites per `HttpClient` when exercising the real session store.
- Prefer ungeocodable sentinel city names (for example `Zzqxnotacity999`) when asserting “not found” paths.

## E2E notes

- Default mode starts **WireMock** (stubbed Open-Meteo JSON) and the **app Docker image** via Testcontainers. The app points `OpenMeteo__GeocodingBaseUrl` / `OpenMeteo__ForecastBaseUrl` at WireMock through `host.docker.internal`.
- Set `E2E_BASE_URL` to run against a deployed site (skips Testcontainers and WireMock).
- Set `E2E_IMAGE` (default `weatherapp:ci`) when using container mode.
- Solution-wide `dotnet test` **excludes** e2e unless `IncludeE2E=true`. CI/CD builds the image, then runs the e2e project with that property.
- Accessibility scenarios are tagged `@a11y` and use **axe-core** (`Deque.AxeCore.Playwright`) with WCAG 2.1 A/AA tags against Search, Forecast, and Favourites (including search results). They run with the rest of the e2e suite in CI/CD.
## Related

- [How to run, build, and test](../how-to/run-build-and-test.md)
- [Vertical Slice Architecture](../explanation/vertical-slice-architecture.md)
