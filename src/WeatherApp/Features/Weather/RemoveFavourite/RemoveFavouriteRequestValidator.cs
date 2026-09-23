using FluentValidation;

namespace WeatherApp.Features.Weather.RemoveFavourite;

public sealed class RemoveFavouriteRequestValidator : AbstractValidator<RemoveFavouriteRequest>
{
    public RemoveFavouriteRequestValidator()
    {
        RuleFor(request => request.City)
            .NotEmpty()
            .WithMessage("A city is required to remove a favourite.");
    }
}
