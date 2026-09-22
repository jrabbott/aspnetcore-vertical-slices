using FluentValidation;

namespace WeatherApp.Features.Weather.Forecast;

public sealed class ForecastRequestValidator : AbstractValidator<ForecastRequest>
{
    public ForecastRequestValidator()
    {
        RuleFor(request => request.City)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Please enter a city name.")
            .MaximumLength(100)
            .WithMessage("City name must be between 1 and 100 characters.");

        RuleFor(request => request.Days)
            .InclusiveBetween(1, 7)
            .WithMessage("Days must be between 1 and 7.");
    }
}
