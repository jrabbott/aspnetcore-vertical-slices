using FluentValidation;
using FluentValidation.Results;
using WeatherApp.Domain.Weather;
using WeatherApp.Infrastructure.Favourites;
using WeatherApp.Infrastructure.Weather;

namespace WeatherApp.Features.Weather.AddFavourite;

public sealed class AddFavouriteHandler(
    IFavouritesStore favouritesStore,
    IWeatherClient weatherClient,
    IValidator<AddFavouriteRequest> validator)
{
    private readonly IFavouritesStore _favouritesStore = favouritesStore;
    private readonly IWeatherClient _weatherClient = weatherClient;
    private readonly IValidator<AddFavouriteRequest> _validator = validator;

    public async Task<AddFavouriteResponse> HandleAsync(
        AddFavouriteRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidationResult validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return AddFavouriteResponse.Fail(validation.Errors[0].ErrorMessage);
        }

        string city = request.City!.Trim();
        WeatherReading? reading = await _weatherClient.GetCurrentAsync(city, cancellationToken);
        return reading is null
            ? AddFavouriteResponse.Fail($"Could not find weather for \"{city}\".")
            : AddFavourite(reading.Location.City);
    }

    private AddFavouriteResponse AddFavourite(string resolvedCity)
    {
        IReadOnlyList<string> favourites = _favouritesStore.GetAll();

        return favourites.Contains(resolvedCity, StringComparer.OrdinalIgnoreCase)
            ? AddFavouriteResponse.Fail($"{resolvedCity} is already in your favourites.")
            : favourites.Count >= FavouritesLimits.MaxCities
            ? AddFavouriteResponse.Fail($"You can save up to {FavouritesLimits.MaxCities} favourites.")
            : _favouritesStore.Add(resolvedCity)
            ? AddFavouriteResponse.Ok($"{resolvedCity} was added to your favourites.")
            : AddFavouriteResponse.Fail($"Could not add {resolvedCity} to your favourites.");
    }
}
