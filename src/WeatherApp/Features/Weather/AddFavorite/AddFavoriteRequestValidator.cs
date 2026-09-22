using FluentValidation;

namespace WeatherApp.Features.Weather.AddFavorite;

public sealed class AddFavoriteRequestValidator : AbstractValidator<AddFavoriteRequest>
{
    public AddFavoriteRequestValidator()
    {
        RuleFor(request => request.City)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Please enter a city name.")
            .MaximumLength(100)
            .WithMessage("City name must be between 1 and 100 characters.");
    }
}
