using Microsoft.AspNetCore.Mvc;

namespace WeatherApp.Features.Weather.AddFavorite;

[Route("weather/favorites/add")]
public sealed class AddFavoriteController(AddFavoriteHandler handler) : Controller
{
    private readonly AddFavoriteHandler _handler = handler;

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(AddFavoriteRequest request, CancellationToken cancellationToken)
    {
        AddFavoriteResponse result = await _handler.HandleAsync(request, cancellationToken);
        TempData["StatusMessage"] = result.Message;
        TempData["StatusIsError"] = !result.Succeeded;
        return RedirectToAction("Index", "Favorites");
    }
}
