namespace RealEstateApp.Core.Interfaces.Services;

public interface IFavoriteService
{
    Task<HashSet<int>> GetFavoritePropertyIdsAsync(string clienteId);

    // true = quedo marcada como favorita, false = se quito, null = la propiedad
    // solicitada no existe.
    Task<bool?> ToggleFavoriteAsync(string clienteId, int propertyId);
}
