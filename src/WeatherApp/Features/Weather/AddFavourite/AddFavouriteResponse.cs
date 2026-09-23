namespace WeatherApp.Features.Weather.AddFavourite;

public sealed class AddFavouriteResponse
{
    public required bool Succeeded
    {
        get; init;
    }
    public required string Message
    {
        get; init;
    }

    public static AddFavouriteResponse Ok(string message)
    {
        return new()
        {
            Succeeded = true,
            Message = message
        };
    }

    public static AddFavouriteResponse Fail(string message)
    {
        return new()
        {
            Succeeded = false,
            Message = message
        };
    }
}
