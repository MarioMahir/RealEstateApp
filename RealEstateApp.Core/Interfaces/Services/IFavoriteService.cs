namespace RealEstateApp.Core.Interfaces.Services;

public interface IFavoriteService
{
    Task<HashSet<int>> GetFavoritePropertyIdsAsync(string clienteId);

    Task<bool?> ToggleFavoriteAsync(string clienteId, int propertyId);
}
