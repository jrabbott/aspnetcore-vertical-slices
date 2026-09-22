using FluentValidation;
using FluentValidation.Results;
using WeatherApp.Domain.Weather;
using WeatherApp.Infrastructure.Favorites;
using WeatherApp.Infrastructure.Weather;

namespace WeatherApp.Features.Weather.AddFavorite;

public sealed class AddFavoriteHandler(
    IFavoritesStore favoritesStore,
    IWeatherClient weatherClient,
    IValidator<AddFavoriteRequest> validator)
{
    private readonly IFavoritesStore _favoritesStore = favoritesStore;
    private readonly IWeatherClient _weatherClient = weatherClient;
    private readonly IValidator<AddFavoriteRequest> _validator = validator;

    public async Task<AddFavoriteResponse> HandleAsync(
        AddFavoriteRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidationResult validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return AddFavoriteResponse.Fail(validation.Errors[0].ErrorMessage);
        }

        string city = request.City!.Trim();
        WeatherReading? reading = await _weatherClient.GetCurrentAsync(city, cancellationToken);

        if (reading is null)
        {
            return AddFavoriteResponse.Fail($"Could not find weather for \"{city}\".");
        }

        bool added = _favoritesStore.Add(reading.Location.City);
        return !added
            ? AddFavoriteResponse.Fail($"{reading.Location.City} is already in your favorites.")
            : AddFavoriteResponse.Ok($"{reading.Location.City} was added to your favorites.");
    }
}
