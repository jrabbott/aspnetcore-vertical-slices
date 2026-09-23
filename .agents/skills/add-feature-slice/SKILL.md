---
name: add-feature-slice
description: Add or extend a Vertical Slice (controller, handler, request/response, optional Razor view) under Features/. Use when creating a new use case, feature slice, MVC page, or command endpoint in this weather sample.
license: MIT
metadata:
  author: weatherapp
  version: "1.0"
---

# Add a feature slice

Create a new use case under `src/WeatherApp/Features/` following this sample’s VSA + MVC pattern.

Before coding, read [docs/how-to/add-a-feature-slice.md](../../../docs/how-to/add-a-feature-slice.md). While implementing, load [references/slice-checklist.md](references/slice-checklist.md).

## Scope

Any feature **area**, not Weather-only:

```text
src/WeatherApp/Features/<Area>/<Slice>/
namespace WeatherApp.Features.<Area>.<Slice>
```

- Default `<Area>` to `Weather` unless the user names another area.
- `Home` is an existing non-Weather peer (`WeatherApp.Features.Home`).
- `FeatureViewLocationExpander` maps any `WeatherApp.Features.*` namespace to `/Features/<Area>/<Slice>/{view}.cshtml`.

## Workflow

1. Decide page slice (renders Razor) vs command slice (redirect + `TempData`, like AddFavourite).
2. Add slice folder, types, and optional `Index.cshtml` per the checklist.
3. Keep the controller thin: bind → handler → `View` or `RedirectToAction`.
4. Register handler + validator explicitly in `src/WeatherApp/Hosting/WeatherAppFeatureServiceCollectionExtensions.cs` (no assembly scanning).
5. Update nav / Tag Helper links if users should open the page.
6. Add unit (± integration) tests; update `FeatureBoundaryTests` slice lists when namespaces change.
7. Run `dotnet test aspnetcore-vertical-slices.slnx` and fix failures.

## Gotchas

- No MediatR, AutoMapper, or generic repositories — handlers are ordinary classes.
- Slice-local request/response types; prefer duplication over sharing with sibling slices.
- Handlers must not depend on `Controller` or sibling feature namespaces.
- Keep `_ViewImports.cshtml` / `_ViewStart.cshtml` at the **web project root** — not under `Views/` alone. See [docs/explanation/razor-view-discovery.md](../../../docs/explanation/razor-view-discovery.md).
- Use **UK spelling** in identifiers and prose (e.g. Favourite, organisation).
- POST commands need `[ValidateAntiForgeryToken]`.

## Related docs

- [Vertical Slice Architecture](../../../docs/explanation/vertical-slice-architecture.md)
- [Routes](../../../docs/reference/routes.md)
- Skill [weatherapp-tests](../weatherapp-tests/SKILL.md) when adding tests
