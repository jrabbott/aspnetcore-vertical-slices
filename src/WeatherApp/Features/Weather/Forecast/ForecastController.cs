using Microsoft.AspNetCore.Mvc;

namespace WeatherApp.Features.Weather.Forecast;

[Route("weather/forecast")]
public sealed class ForecastController : Controller
{
    private readonly ForecastHandler _handler;

    public ForecastController(ForecastHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] ForecastRequest request, CancellationToken cancellationToken)
    {
        var searched = Request.Query.ContainsKey(nameof(ForecastRequest.City));
        var response = await _handler.HandleAsync(request, searched, cancellationToken);
        return View(response);
    }
}
