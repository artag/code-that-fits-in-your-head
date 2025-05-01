namespace Restaurant.RestApi.Options;

public class OptionsRestaurantDatabase : IRestaurantDatabase
{
    public Task<IEnumerable<string>> GetAllNames()
    {
        var rs = new[] { "Hipgnosta", "Nono", "The Vatican Cellar" };
        return Task.FromResult(rs.AsEnumerable());
    }

    public Task<int?> GetId(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        return Task.FromResult((int?)name.Length);
    }

    public Task<string?> GetName(int id)
    {
        var name = "Hipgnosta";
        if (id == 4)
            name = "Nono";
        if (id == 18)
            name = "The Vatican Cellar";

        return Task.FromResult((string?)name);
    }
}
