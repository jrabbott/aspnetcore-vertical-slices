using Microsoft.AspNetCore.Mvc;

namespace WeatherApp.Features.Weather.Favourites;

[Route("weather/favourites")]
public sealed class FavouritesController(FavouritesHandler handler) : Controller
{
    private readonly FavouritesHandler _handler = handler;

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        string? statusMessage = TempData["StatusMessage"] as string;
        bool statusIsError = TempData["StatusIsError"] as bool? ?? false;

        FavouritesResponse response = await _handler.HandleAsync(
            new FavouritesRequest(),
            statusMessage,
            statusIsError,
            cancellationToken);

        return View(response);
    }
}
