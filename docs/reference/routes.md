# Routes and endpoints

HTTP surface of the weather sample. Controllers use attribute routing under `src/WeatherApp/Features/`.

| Use case | Method | Route | Controller / endpoint | Renders view? |
|---|---|---|---|---|
| Home redirect | GET | `/` | Minimal `MapGet` in hosting | No — redirects to Search |
| Search | GET | `/weather/search` | `SearchController` | Yes — `Features/Weather/Search/Index.cshtml` |
| Forecast | GET | `/weather/forecast` | `ForecastController` | Yes — `Features/Weather/Forecast/Index.cshtml` |
| Favourites | GET | `/weather/favourites` | `FavouritesController` | Yes — `Features/Weather/Favourites/Index.cshtml` |
| Add favourite | POST | `/weather/favourites/add` | `AddFavouriteController` | No — redirects to Favourites |
| Remove favourite | POST | `/weather/favourites/remove` | `RemoveFavouriteController` | No — redirects to Favourites |
| Error | GET | `/Home/Error` | `HomeController` | Yes — shared error page (Production exception handler) |

## Notes

- Root `/` is registered in `WeatherAppApplicationBuilderExtensions.MapWeatherAppEndpoints` (not `HomeController`).
- Search and Forecast take `city` as a query string bound to the slice request model.
- Add / Remove require an antiforgery token (global `AutoValidateAntiforgeryToken` plus form tokens) and a form `City` field.
- Command slices set `TempData["StatusMessage"]` / `TempData["StatusIsError"]` for flash messaging on Favourites.
- AddFavourite and RemoveFavourite demonstrate that **not every vertical slice is a page**.

## Related

- [How to add a feature slice](../how-to/add-a-feature-slice.md)
- [Tutorial](../tutorials/run-the-weather-app.md)
