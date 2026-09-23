namespace WeatherApp.Infrastructure.Favourites;

public interface IFavouritesStore
{
    public IReadOnlyList<string> GetAll();
    public bool Add(string city);
    public bool Remove(string city);
}
