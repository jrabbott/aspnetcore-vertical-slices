using Microsoft.AspNetCore.Mvc;

namespace WeatherApp.Features.Weather.Favorites;

[Route("weather/favorites")]
public sealed class FavoritesController : Controller
{
    private readonly FavoritesHandler _handler;

    public FavoritesController(FavoritesHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var statusMessage = TempData["StatusMessage"] as string;
        var statusIsError = TempData["StatusIsError"] as bool? ?? false;

        var response = await _handler.HandleAsync(
            new FavoritesRequest(),
            statusMessage,
            statusIsError,
            cancellationToken);

        return View(response);
    }
}
