namespace Restaurant.RestApi.Options;

public class OptionsRestaurantDatabase : IRestaurantDatabase
{
    private readonly RestaurantOptions[] _restaurants;

    internal OptionsRestaurantDatabase(RestaurantOptions[] restaurants)
    {
        _restaurants = restaurants;
    }

    public Task<IEnumerable<string>> GetAllNames()
    {
        var rs = _restaurants.Select(r => r.Name).OfType<string>();
        return Task.FromResult(rs.AsEnumerable());
    }

    public Task<int?> GetId(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        var rs = _restaurants
            .Where(r => r.Name == name)
            .Select(r => (int?)r.Id);
        var r = rs.SingleOrDefault();
        return Task.FromResult(r);
    }

    public Task<string?> GetName(int id)
    {
        var rs = _restaurants
            .Where(r => r.Id == id)
            .Select(r => r.Name);
        var r = rs.SingleOrDefault();
        return Task.FromResult(r);
    }
}
