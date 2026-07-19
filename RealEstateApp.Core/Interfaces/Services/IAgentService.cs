using RealEstateApp.Core.DTOs.Agent;
using RealEstateApp.Core.DTOs.Property;

namespace RealEstateApp.Core.Interfaces.Services;

public interface IAgentService
{
    Task<List<AgentDto>> GetAllAsync();

    // null = el usuario no existe o no tiene rol Agente.
    Task<AgentDto?> GetByIdAsync(string id);

    // null = el agente no existe; lista vacia = existe pero sin propiedades.
    Task<List<PropertyDto>?> GetAgentPropertiesAsync(string agentId);

    // false = el agente no existe.
    Task<bool> ChangeStatusAsync(string agentId, bool activo);
}
