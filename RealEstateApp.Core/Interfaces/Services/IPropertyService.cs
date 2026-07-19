using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Models;

namespace RealEstateApp.Core.Interfaces.Services;

public interface IPropertyService : IGenericService<Property>
{
    // Devuelven la entidad con PropertyType/SaleType/Agent/Mejoras ya cargados
    // (Include), lista para mapear a PropertyDto en el controlador. El
    // IGenericService.GetByIdAsync heredado NO carga esas relaciones.
    Task<List<Property>> GetAllWithDetailsAsync();
    Task<Property?> GetByIdWithDetailsAsync(int id);
    Task<Property?> GetByCodeWithDetailsAsync(string codigo);

    // Superficie publica (Home, Agentes): siempre limitada a Estado Disponible,
    // ordenada de mas reciente a mas antigua, con los mismos filtros
    // combinables en cualquier pantalla que la use.
    Task<List<Property>> SearchAvailableAsync(PropertyFilterCriteria criteria);
    Task<Property?> GetAvailableByIdAsync(int id);
    Task<Property?> GetAvailableByCodeAsync(string codigo);

    // criteria nulo = sin filtros (todas las disponibles del agente). La
    // pantalla "propiedades de un agente" reutiliza los mismos filtros que el
    // Home publico sobre este mismo metodo.
    Task<List<Property>> GetAvailableByAgentAsync(string agentId, PropertyFilterCriteria? criteria = null);

    // Home del agente (Etapa 2, contenido minimo -- Etapa 4 agrega alta/edicion).
    Task<List<Property>> GetAllByAgentAsync(string agentId);

    // "Mis propiedades" (Cliente, Etapa 3): favoritos que siguen Disponible.
    Task<List<Property>> GetFavoritesByClienteAsync(string clienteId);

    // Mantenimiento de propiedades (Agente, Etapa 4). null = no existe o no
    // pertenece a este agente; el llamador decide si el Estado actual permite
    // la accion (editar/eliminar exigen Disponible, Detalle permite cualquiera).
    Task<Property?> GetByIdForAgentAsync(int id, string agentId);

    // 6 digitos, unico, generado por el sistema -- nunca por el usuario.
    Task<string> GenerateUniqueCodeAsync();

    Task<Property> CreatePropertyAsync(Property property, List<int> improvementIds, List<string> imageUrls);

    // false = la propiedad no existe, no pertenece a este agente, o ya no esta Disponible.
    Task<bool> UpdatePropertyAsync(
        Property cambios, List<int> improvementIds, List<int> imageIdsToRemove, List<string> newImageUrls);

    Task<bool> DeletePropertyAsync(int propertyId, string agentId);
}
