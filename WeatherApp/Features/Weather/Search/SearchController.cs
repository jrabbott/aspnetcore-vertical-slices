using Microsoft.AspNetCore.Mvc;

namespace WeatherApp.Features.Weather.Search;

[Route("weather/search")]
public sealed class SearchController : Controller
{
    private readonly SearchHandler _handler;

    public SearchController(SearchHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] SearchRequest request, CancellationToken cancellationToken)
    {
        var searched = Request.Query.ContainsKey(nameof(SearchRequest.City));
        var response = await _handler.HandleAsync(request, searched, cancellationToken);
        return View(response);
    }
}
