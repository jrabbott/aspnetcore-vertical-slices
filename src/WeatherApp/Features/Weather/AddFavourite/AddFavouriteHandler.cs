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

        if (reading is null)
        {
            return AddFavouriteResponse.Fail($"Could not find weather for \"{city}\".");
        }

        bool added = _favouritesStore.Add(reading.Location.City);
        return !added
            ? AddFavouriteResponse.Fail($"{reading.Location.City} is already in your favourites.")
            : AddFavouriteResponse.Ok($"{reading.Location.City} was added to your favourites.");
    }
}
