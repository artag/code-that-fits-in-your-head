namespace Restaurant.RestApi.Options;

public class OptionsRestaurantDatabase : IRestaurantDatabase
{
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
