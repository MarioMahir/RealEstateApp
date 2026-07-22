using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Models;

namespace RealEstateApp.Core.Interfaces.Services;

public interface IPropertyService : IGenericService<Property>
{
    Task<List<Property>> GetAllWithDetailsAsync();
    Task<Property?> GetByIdWithDetailsAsync(int id);
    Task<Property?> GetByCodeWithDetailsAsync(string codigo);

    Task<List<Property>> SearchAvailableAsync(PropertyFilterCriteria criteria);
    Task<Property?> GetAvailableByIdAsync(int id);
    Task<Property?> GetAvailableByCodeAsync(string codigo);

    Task<List<Property>> GetAvailableByAgentAsync(string agentId, PropertyFilterCriteria? criteria = null);

    Task<List<Property>> GetAllByAgentAsync(string agentId);

    Task<List<Property>> GetFavoritesByClienteAsync(string clienteId);

    Task<Property?> GetByIdForAgentAsync(int id, string agentId);

    Task<string> GenerateUniqueCodeAsync();

    Task<Property> CreatePropertyAsync(Property property, List<int> improvementIds, List<string> imageUrls);

    Task<bool> UpdatePropertyAsync(
        Property cambios, List<int> improvementIds, List<int> imageIdsToRemove, List<string> newImageUrls);

    Task<bool> DeletePropertyAsync(int propertyId, string agentId);
}
