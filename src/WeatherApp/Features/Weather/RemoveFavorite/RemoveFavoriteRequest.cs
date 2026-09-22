using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Features.Weather.RemoveFavorite;

public sealed class RemoveFavoriteRequest
{
    [Required]
    public string? City { get; set; }
}
