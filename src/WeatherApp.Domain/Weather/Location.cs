namespace WeatherApp.Domain.Weather;

public sealed class Location
{
    public required string City
    {
        get; init;
    }
    public required string Country
    {
        get; init;
    }
}
