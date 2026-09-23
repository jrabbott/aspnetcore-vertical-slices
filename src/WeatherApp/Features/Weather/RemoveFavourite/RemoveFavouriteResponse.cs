namespace WeatherApp.Features.Weather.RemoveFavourite;

public sealed class RemoveFavouriteResponse
{
    public required bool Succeeded
    {
        get; init;
    }
    public required string Message
    {
        get; init;
    }

    public static RemoveFavouriteResponse Ok(string message)
    {
        return new()
        {
            Succeeded = true,
            Message = message
        };
    }

    public static RemoveFavouriteResponse Fail(string message)
    {
        return new()
        {
            Succeeded = false,
            Message = message
        };
    }
}
