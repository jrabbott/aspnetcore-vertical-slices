using FluentValidation;

namespace WeatherApp.Features.Weather.RemoveFavourite;

public sealed class RemoveFavouriteRequestValidator : AbstractValidator<RemoveFavouriteRequest>
{
    public RemoveFavouriteRequestValidator()
    {
        RuleFor(request => request.City)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("A city is required to remove a favourite.")
            .MaximumLength(100)
            .WithMessage("City name must be between 1 and 100 characters.");
    }
}
