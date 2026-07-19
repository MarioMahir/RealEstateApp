using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Constants;
using RealEstateApp.Core.DTOs.Agent;
using RealEstateApp.Core.Interfaces.Services;

namespace RealEstateApp.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{Roles.Administrador},{Roles.Desarrollador}")]
public class AgentsController : ControllerBase
{
    private readonly IAgentService _agentService;

    public AgentsController(IAgentService agentService)
    {
        _agentService = agentService;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var agents = await _agentService.GetAllAsync();
        return agents.Count == 0 ? NoContent() : Ok(agents);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var agent = await _agentService.GetByIdAsync(id);
        return agent is null
            ? NotFound(new { message = "El agente solicitado no existe." })
            : Ok(agent);
    }

    [HttpGet("{id}/properties")]
    public async Task<IActionResult> GetAgentProperty(string id)
    {
        var properties = await _agentService.GetAgentPropertiesAsync(id);
        if (properties is null)
            return NotFound(new { message = "El agente solicitado no existe." });

        return properties.Count == 0 ? NoContent() : Ok(properties);
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<IActionResult> ChangeStatus(string id, AgentStatusUpdateDto dto)
    {
        // dto.Estado es bool? justo para que [Required] detecte un cuerpo sin
        // el campo (en un bool no-nullable, [Required] es un no-op); si llego
        // aqui, ApiController ya garantizo que no es null.
        var updated = await _agentService.ChangeStatusAsync(id, dto.Estado!.Value);
        return updated
            ? NoContent()
            : NotFound(new { message = "El agente solicitado no existe." });
    }
}
