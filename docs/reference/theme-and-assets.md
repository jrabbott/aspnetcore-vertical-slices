# Theme and front-end assets

Static presentation assets for the MVC host. There is no npm, LibMan, or CSS framework package — the UI is hand-authored CSS plus a small progressive-enhancement script.

## Stylesheets

Linked from `Views/Shared/_Layout.cshtml` in order (each with `asp-append-version`):

| File | Role |
|---|---|
| `wwwroot/css/tokens.css` | Design tokens (`:root` custom properties: colour, radius, type, shadow) |
| `wwwroot/css/base.css` | Document defaults, shell chrome (header, nav, footer) |
| `wwwroot/css/components.css` | Page sections, forms, buttons, alerts, weather / forecast / favourites UI |

Extend the theme by adjusting tokens first; add component rules only when markup needs a new pattern. Prefer UK spelling in class names that encode product language (`favourites-list`, not `favorites-list`).

## Scripts

| File | Role |
|---|---|
| `wwwroot/js/site.js` | Shared progressive enhancement for favourite command forms; no-op when `fetch` / `FormData` are unavailable |

- Loaded with `defer` so HTML parses without blocking on script.
- Optional per-view scripts use `@section Scripts` in the layout.
- Core flows must remain usable with JavaScript disabled.

## Markup hooks

- `data-enhance="favourite-command"` — opt a POST form into JSON enhancement.
- `data-on-success` — `flash` \| `remove-row` \| `reload` after a successful JSON response.

## Related

- [Progressive enhancement](../explanation/progressive-enhancement.md)
- [Repository layout](repository-layout.md)
- [Routes](routes.md)
