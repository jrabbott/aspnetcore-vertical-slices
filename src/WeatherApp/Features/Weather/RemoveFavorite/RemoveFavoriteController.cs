using Microsoft.AspNetCore.Mvc;

namespace WeatherApp.Features.Weather.RemoveFavorite;

[Route("weather/favorites/remove")]
public sealed class RemoveFavoriteController(RemoveFavoriteHandler handler) : Controller
{
    private readonly RemoveFavoriteHandler _handler = handler;

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(RemoveFavoriteRequest request, CancellationToken cancellationToken)
    {
        RemoveFavoriteResponse result = await _handler.HandleAsync(request, cancellationToken);
        TempData["StatusMessage"] = result.Message;
        TempData["StatusIsError"] = !result.Succeeded;
        return RedirectToAction("Index", "Favorites");
    }
}
