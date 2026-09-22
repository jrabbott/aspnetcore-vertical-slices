using Microsoft.AspNetCore.Http;
using WeatherApp.Favorites;

namespace WeatherApp.Unit.Tests;

public sealed class SessionFavoritesStoreTests
{
    [Fact]
    public void Add_ThenGetAll_ReturnsSortedCities()
    {
        SessionFavoritesStore store = CreateStore();

        Assert.True(store.Add("Tokyo"));
        Assert.True(store.Add("London"));

        Assert.Equal(["London", "Tokyo"], store.GetAll());
    }

    [Fact]
    public void Add_DuplicateCity_ReturnsFalse()
    {
        SessionFavoritesStore store = CreateStore();
        Assert.True(store.Add("Paris"));

        Assert.False(store.Add("paris"));
        Assert.Equal(["Paris"], store.GetAll());
    }

    [Fact]
    public void Remove_ExistingCity_ReturnsTrue()
    {
        SessionFavoritesStore store = CreateStore();
        Assert.True(store.Add("London"));
        Assert.True(store.Add("Tokyo"));

        Assert.True(store.Remove("London"));
        Assert.Equal(["Tokyo"], store.GetAll());
    }

    [Fact]
    public void StartsEmpty()
    {
        SessionFavoritesStore store = CreateStore();

        Assert.Empty(store.GetAll());
    }

    private static SessionFavoritesStore CreateStore()
    {
        var context = new DefaultHttpContext
        {
            Session = new FakeSession()
        };
        var accessor = new HttpContextAccessor { HttpContext = context };
        return new SessionFavoritesStore(accessor);
    }

    private sealed class FakeSession : ISession
    {
        private readonly Dictionary<string, byte[]> _store = new(StringComparer.Ordinal);

        public string Id { get; } = "test-session";

        public bool IsAvailable => true;

        public IEnumerable<string> Keys => _store.Keys;

        public void Clear()
        {
            _store.Clear();
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task LoadAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Remove(string key)
        {
            _store.Remove(key);
        }

        public void Set(string key, byte[] value)
        {
            _store[key] = value;
        }

        public bool TryGetValue(string key, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out byte[]? value)
        {
            if (_store.TryGetValue(key, out byte[]? found))
            {
                value = found;
                return true;
            }

            value = null;
            return false;
        }
    }
}
