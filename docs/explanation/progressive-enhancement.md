# Progressive enhancement

Why WeatherApp treats JavaScript as an optional improvement on a working HTML baseline — not as the primary delivery mechanism that must be patched when it fails.

## Layers

1. **HTML** — every core flow is a normal GET or POST that works with forms and links alone.
2. **CSS** — presentation lives in a small design-system stylesheet stack (`tokens` → `base` → `components`). Layout and readability do not require script.
3. **JavaScript** — when `fetch` and `FormData` exist, favourite add/remove forms may request JSON and update the page without a full navigation. If script is missing or the request fails, the same form submits classically (redirect + `TempData`).

That order is **progressive enhancement**. The opposite approach — ship a JS-first UI and then try to recover when script breaks — is **graceful degradation**.

## Favourite commands

| Without JS | With JS |
|---|---|
| POST form → antiforgery → handler → redirect to Favourites → flash via `TempData` | Same POST with `Accept: application/json` → JSON `{ succeeded, message }` → status alert / row removal / reload |

Forms opt in with `data-enhance="favourite-command"` and a `data-on-success` hint (`flash`, `remove-row`, or `reload`). Shared behaviour lives in `wwwroot/js/site.js` (loaded with `defer`). Slice-specific scripts can still use the layout `Scripts` section.

## Design choice

The sample stays an MVC + Razor teaching app. Optional fetch keeps the command slices honest (same handlers, same antiforgery) while demonstrating that SPA-style round-trips are an enhancement, not a requirement.

## Related

- [Theme and front-end assets](../reference/theme-and-assets.md)
- [Routes](../reference/routes.md)
- [Design choices](design-choices.md)
