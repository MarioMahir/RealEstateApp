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
}
