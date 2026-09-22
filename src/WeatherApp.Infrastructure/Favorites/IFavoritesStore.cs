namespace WeatherApp.Infrastructure.Favorites;

public interface IFavoritesStore
{
    public IReadOnlyList<string> GetAll();
    public bool Add(string city);
    public bool Remove(string city);
}
