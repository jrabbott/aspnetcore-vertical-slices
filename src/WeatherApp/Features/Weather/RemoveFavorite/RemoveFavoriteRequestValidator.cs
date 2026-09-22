using FluentValidation;

namespace WeatherApp.Features.Weather.RemoveFavorite;

public sealed class RemoveFavoriteRequestValidator : AbstractValidator<RemoveFavoriteRequest>
{
    public RemoveFavoriteRequestValidator()
    {
        RuleFor(request => request.City)
            .NotEmpty()
            .WithMessage("A city is required to remove a favorite.");
    }
}
