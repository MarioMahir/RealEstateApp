using RealEstateApp.Core.DTOs.Agent;
using RealEstateApp.Core.DTOs.Property;
using RealEstateApp.Core.Entities;

namespace RealEstateApp.Core.Interfaces.Services;

public interface IAgentService
{
    Task<List<AgentDto>> GetAllAsync();

    Task<AgentDto?> GetByIdAsync(string id);

    Task<List<PropertyDto>?> GetAgentPropertiesAsync(string agentId);

    Task<bool> ChangeStatusAsync(string agentId, bool activo);

    Task<List<ApplicationUser>> GetActiveAgentsAsync(string? nombreBusqueda);

    Task<ApplicationUser?> GetActiveAgentByIdAsync(string id);

    Task<bool> DeleteAgentAsync(string agentId);
}
