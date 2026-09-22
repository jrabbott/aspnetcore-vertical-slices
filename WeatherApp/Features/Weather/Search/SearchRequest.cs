using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Features.Weather.Search;

public sealed class SearchRequest
{
    [Display(Name = "City")]
    [Required(ErrorMessage = "Please enter a city name.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "City name must be between 1 and 100 characters.")]
    public string? City { get; set; }
}
