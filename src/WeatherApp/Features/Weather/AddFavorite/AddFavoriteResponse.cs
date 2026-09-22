namespace WeatherApp.Features.Weather.AddFavorite;

public sealed class AddFavoriteResponse
{
    public required bool Succeeded { get; init; }
    public required string Message { get; init; }

    public static AddFavoriteResponse Ok(string message) =>
        new() { Succeeded = true, Message = message };

    public static AddFavoriteResponse Fail(string message) =>
        new() { Succeeded = false, Message = message };
}
