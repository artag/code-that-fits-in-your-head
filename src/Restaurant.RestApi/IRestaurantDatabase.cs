namespace Restaurant.RestApi;

public interface IRestaurantDatabase
{
    Task<string?> GetName(int id);
}
