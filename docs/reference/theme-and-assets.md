# Theme and front-end assets

Presentation assets for the MVC host. There is no npm, LibMan, or CSS framework package. Sources are **SCSS** and **TypeScript**; `dotnet build` compiles them into `wwwroot` via NuGet MSBuild tools (`AspNetCore.SassCompiler`, `Microsoft.TypeScript.MSBuild`).

## Sources vs outputs

| Source | Output (served) |
|---|---|
| `Styles/_variables.scss` | (partial — not emitted) |
| `Styles/tokens.scss` | `wwwroot/css/tokens.css` |
| `Styles/base.scss` | `wwwroot/css/base.css` |
| `Styles/components.scss` | `wwwroot/css/components.css` |
| `Scripts/site.ts` | `wwwroot/js/site.js` |

Configuration: `sasscompiler.json` (Styles → `wwwroot/css`) and `tsconfig.json` (Scripts → `wwwroot/js`).

**Sources only in git.** Generated `wwwroot/css/*.css` and `wwwroot/js/*.js` are gitignored; `dotnet build` recreates them and registers them for MapStaticAssets. Keep `wwwroot/favicon.ico` (and other hand-authored static files) tracked. Empty `wwwroot/css/.gitkeep` and `wwwroot/js/.gitkeep` preserve the output folders.

## Stylesheets

Linked from `Views/Shared/_Layout.cshtml` in order (each with `asp-append-version`):

| File | Role |
|---|---|
| `wwwroot/css/tokens.css` | Design tokens (`:root` custom properties: colour, radius, type, shadow) |
| `wwwroot/css/base.css` | Document defaults, shell chrome (header, nav, footer) |
| `wwwroot/css/components.css` | Page sections, forms, buttons, alerts, weather / forecast / favourites UI |

Extend the theme by adjusting `_variables.scss` / `tokens.scss` first; add component rules only when markup needs a new pattern. Prefer UK spelling in class names that encode product language (`favourites-list`, not `favorites-list`).

## Scripts

| File | Role |
|---|---|
| `Scripts/site.ts` (→ `wwwroot/js/site.js`) | Shared progressive enhancement for favourite command forms; no-op when `fetch` / `FormData` are unavailable |

- Loaded with `defer` so HTML parses without blocking on script.
- Optional per-view scripts use `@section Scripts` in the layout.
- Core flows must remain usable with JavaScript disabled.

## Markup hooks

- `data-enhance="favourite-command"` — opt a POST form into JSON enhancement.
- `data-on-success` — `flash` \| `remove-row` \| `reload` after a successful JSON response.

## Related

- [Progressive enhancement](../explanation/progressive-enhancement.md)
- [Build system and packages](build-and-packages.md)
- [Repository layout](repository-layout.md)
- [Routes](routes.md)
