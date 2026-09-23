using Microsoft.AspNetCore.Mvc;
using WeatherApp.Mvc;

namespace WeatherApp.Features.Weather.AddFavourite;

[Route("weather/favourites/add")]
public sealed class AddFavouriteController(AddFavouriteHandler handler) : Controller
{
    private readonly AddFavouriteHandler _handler = handler;

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(AddFavouriteRequest request, CancellationToken cancellationToken)
    {
        AddFavouriteResponse result = await _handler.HandleAsync(request, cancellationToken);

        if (RequestAccepts.Json(Request))
        {
            return Json(result);
        }

        TempData["StatusMessage"] = result.Message;
        TempData["StatusIsError"] = !result.Succeeded;
        return RedirectToAction("Index", "Favourites");
    }
}
