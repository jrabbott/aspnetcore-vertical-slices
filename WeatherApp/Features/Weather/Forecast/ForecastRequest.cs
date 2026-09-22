using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Features.Weather.Forecast;

public sealed class ForecastRequest
{
    [Display(Name = "City")]
    [Required(ErrorMessage = "Please enter a city name.")]
    [StringLength(100, MinimumLength = 1)]
    public string? City { get; set; }

    [Range(1, 7)]
    public int Days { get; set; } = 5;
}
