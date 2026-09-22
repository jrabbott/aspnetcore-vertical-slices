using Microsoft.AspNetCore.Mvc;

namespace WeatherApp.Features.Weather.RemoveFavorite;

[Route("weather/favorites/remove")]
public sealed class RemoveFavoriteController : Controller
{
    private readonly RemoveFavoriteHandler _handler;

    public RemoveFavoriteController(RemoveFavoriteHandler handler)
    {
        _handler = handler;
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(RemoveFavoriteRequest request, CancellationToken cancellationToken)
    {
        var result = await _handler.HandleAsync(request, cancellationToken);
        TempData["StatusMessage"] = result.Message;
        TempData["StatusIsError"] = !result.Succeeded;
        return RedirectToAction("Index", "Favorites");
    }
}
