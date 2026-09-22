using FluentValidation;

namespace WeatherApp.Features.Weather.Search;

public sealed class SearchRequestValidator : AbstractValidator<SearchRequest>
{
    public SearchRequestValidator()
    {
        RuleFor(request => request.City)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Please enter a city name.")
            .MaximumLength(100)
            .WithMessage("City name must be between 1 and 100 characters.");
    }
}
