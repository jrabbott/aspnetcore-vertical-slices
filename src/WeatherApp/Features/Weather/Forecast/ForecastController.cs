using Microsoft.AspNetCore.Mvc;

namespace WeatherApp.Features.Weather.Forecast;

[Route("weather/forecast")]
public sealed class ForecastController(ForecastHandler handler) : Controller
{
    private readonly ForecastHandler _handler = handler;

    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] ForecastRequest request, CancellationToken cancellationToken)
    {
        bool searched = Request.Query.ContainsKey(nameof(ForecastRequest.City));
        ForecastResponse response = await _handler.HandleAsync(request, searched, cancellationToken);
        return View(response);
    }
}
