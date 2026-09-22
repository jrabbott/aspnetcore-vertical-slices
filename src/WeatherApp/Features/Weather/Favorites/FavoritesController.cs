using Microsoft.AspNetCore.Mvc;

namespace WeatherApp.Features.Weather.Favorites;

[Route("weather/favorites")]
public sealed class FavoritesController(FavoritesHandler handler) : Controller
{
    private readonly FavoritesHandler _handler = handler;

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        string? statusMessage = TempData["StatusMessage"] as string;
        bool statusIsError = TempData["StatusIsError"] as bool? ?? false;

        FavoritesResponse response = await _handler.HandleAsync(
            new FavoritesRequest(),
            statusMessage,
            statusIsError,
            cancellationToken);

        return View(response);
    }
}
