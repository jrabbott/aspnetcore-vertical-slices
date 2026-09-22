using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Features.Weather.AddFavorite;

public sealed class AddFavoriteRequest
{
    [Required(ErrorMessage = "Please enter a city name.")]
    [StringLength(100, MinimumLength = 1)]
    public string? City { get; set; }
}
