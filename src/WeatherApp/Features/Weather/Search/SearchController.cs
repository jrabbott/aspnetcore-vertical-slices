using Microsoft.AspNetCore.Mvc;

namespace WeatherApp.Features.Weather.Search;

[Route("weather/search")]
public sealed class SearchController(SearchHandler handler) : Controller
{
    private readonly SearchHandler _handler = handler;

    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] SearchRequest request, CancellationToken cancellationToken)
    {
        bool searched = Request.Query.ContainsKey(nameof(SearchRequest.City));
        SearchResponse response = await _handler.HandleAsync(request, searched, cancellationToken);
        return View(response);
    }
}
