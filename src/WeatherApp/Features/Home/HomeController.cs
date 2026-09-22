using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WeatherApp.Features.Home;

public sealed class HomeController : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}

public sealed class ErrorViewModel
{
    public string? RequestId
    {
        get; init;
    }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
