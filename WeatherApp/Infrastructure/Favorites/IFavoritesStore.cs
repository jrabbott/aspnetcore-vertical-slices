namespace WeatherApp.Infrastructure.Favorites;

public interface IFavoritesStore
{
    IReadOnlyList<string> GetAll();
    bool Add(string city);
    bool Remove(string city);
}
