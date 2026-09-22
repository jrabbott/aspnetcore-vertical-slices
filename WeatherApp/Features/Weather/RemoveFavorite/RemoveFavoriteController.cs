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
    public IActionResult Index(RemoveFavoriteRequest request)
    {
        var result = _handler.Handle(request);
        TempData["StatusMessage"] = result.Message;
        TempData["StatusIsError"] = !result.Succeeded;
        return RedirectToAction("Index", "Favorites");
    }
}
