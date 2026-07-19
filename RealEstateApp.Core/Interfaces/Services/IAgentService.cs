using RealEstateApp.Core.DTOs.Agent;
using RealEstateApp.Core.DTOs.Property;
using RealEstateApp.Core.Entities;

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

    // Superficie publica de la WebApp (pantalla "Agentes"): solo Activos,
    // orden alfabetico, con busqueda opcional por nombre/apellido.
    Task<List<ApplicationUser>> GetActiveAgentsAsync(string? nombreBusqueda);

    // null = no existe o esta Inactivo (la pantalla publica lo trata igual).
    Task<ApplicationUser?> GetActiveAgentByIdAsync(string id);
}
