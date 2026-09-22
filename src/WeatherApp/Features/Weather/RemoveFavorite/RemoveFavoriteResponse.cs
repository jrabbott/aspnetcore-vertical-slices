namespace WeatherApp.Features.Weather.RemoveFavorite;

public sealed class RemoveFavoriteResponse
{
    public required bool Succeeded
    {
        get; init;
    }
    public required string Message
    {
        get; init;
    }

    public static RemoveFavoriteResponse Ok(string message)
    {
        return new()
        {
            Succeeded = true,
            Message = message
        };
    }

    public static RemoveFavoriteResponse Fail(string message)
    {
        return new()
        {
            Succeeded = false,
            Message = message
        };
    }
}
