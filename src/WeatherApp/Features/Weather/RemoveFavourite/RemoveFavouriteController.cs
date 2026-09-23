using Microsoft.AspNetCore.Mvc;

namespace WeatherApp.Features.Weather.RemoveFavourite;

[Route("weather/favourites/remove")]
public sealed class RemoveFavouriteController(RemoveFavouriteHandler handler) : Controller
{
    private readonly RemoveFavouriteHandler _handler = handler;

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(RemoveFavouriteRequest request, CancellationToken cancellationToken)
    {
        RemoveFavouriteResponse result = await _handler.HandleAsync(request, cancellationToken);
        TempData["StatusMessage"] = result.Message;
        TempData["StatusIsError"] = !result.Succeeded;
        return RedirectToAction("Index", "Favourites");
    }
}
