# Run the weather app

This tutorial walks you through the sample once, end to end. By the end you will have restored the solution, run the site, searched for weather, added a favourite, and confirmed the tests pass.

You do **not** need to understand Vertical Slice Architecture yet. Follow the steps; explanations come later in [Explanation](../explanation/vertical-slice-architecture.md).

## What you need

- [.NET 10 SDK](https://dotnet.microsoft.com/download) installed (`dotnet --version` shows a 10.x SDK)

## 1. Restore and run

From the repository root:

```bash
dotnet restore aspnetcore-vertical-slices.slnx
dotnet run --project src/WeatherApp
```

When the host starts, the console prints a local URL (often `http://localhost:5xxx`). Open that URL in a browser.

You should land on **Search** (root `/` redirects there).

## 2. Search for a city

1. On Search, enter `London` and submit (or open `/weather/search?city=London`).
2. Confirm you see current weather for London from Open-Meteo.
3. Open **Forecast** in the nav, or go to `/weather/forecast?city=Paris`, and confirm a multi-day forecast appears.

If a city cannot be geocoded, the page shows a clear message instead of crashing.

## 3. Save a favourite

1. Open **Favourites** (`/weather/favourites`).
2. Add `Tokyo` (form or suggestion chip) and submit.
3. Confirm Tokyo appears in your list with weather when available.
4. Optionally open Search from that row, or remove the city.

Favourites are stored **per browser session**. A second browser (or a cleared session) starts with an empty list — that is expected.

## 4. Run the tests

In another terminal (you can leave the app running):

```bash
dotnet test aspnetcore-vertical-slices.slnx
```

All projects under `tests/` should pass. That includes architecture rules that keep feature slices isolated.

## What you just saw

| You did | The sample demonstrates |
|---|---|
| Search / Forecast pages | Query slices with feature-local Razor views |
| Add / remove favourite | Command slices that redirect (no dedicated view) |
| Session-scoped list | Web concerns stay in the host; handlers use `IFavouritesStore` |
| `dotnet test` | Unit, integration, and architecture tests as a safety net |

## Next steps

- **Want to change the sample?** [Contributing](../../CONTRIBUTING.md) and [Add a feature slice](../how-to/add-a-feature-slice.md)
- **Task-oriented:** [Add a feature slice](../how-to/add-a-feature-slice.md), [Run, build, and test](../how-to/run-build-and-test.md)
- **Understand the design:** [Vertical Slice Architecture](../explanation/vertical-slice-architecture.md)
- **Look up details:** [Routes](../reference/routes.md), [Repository layout](../reference/repository-layout.md)
