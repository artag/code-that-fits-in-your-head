namespace Restaurant.RestApi;

public interface IRestaurantDatabase
{
    Task<string?> GetName(int id);
    Task<int?> GetId(string name);
    Task<IEnumerable<string>> GetAllNames();
}
