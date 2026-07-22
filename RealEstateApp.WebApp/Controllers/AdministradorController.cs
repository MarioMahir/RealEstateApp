using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Constants;
using RealEstateApp.Core.Interfaces.Services;
using RealEstateApp.Core.ViewModels.Administrador;

namespace RealEstateApp.WebApp.Controllers;

[Authorize(Roles = Roles.Administrador)]
public class AdministradorController : Controller
{
    private readonly IAdministradorService _administradorService;
    private readonly IAgentService _agentService;
    private readonly IMapper _mapper;

    public AdministradorController(
        IAdministradorService administradorService, IAgentService agentService, IMapper mapper)
    {
        _administradorService = administradorService;
        _agentService = agentService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        var indicadores = await _administradorService.GetIndicadoresAsync();
        return View(indicadores);
    }

    public async Task<IActionResult> Agentes()
    {
        var agentes = await _agentService.GetAllAsync();
        var modelo = _mapper.Map<List<AgentAdminListItemViewModel>>(
            agentes.OrderBy(a => a.Nombre).ThenBy(a => a.Apellido));

        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAgenteEstado(string id)
    {
        var agente = await _agentService.GetByIdAsync(id);
        if (agente is null)
        {
            TempData["Error"] = "El agente seleccionado no existe.";
            return RedirectToAction(nameof(Agentes));
        }

        var activar = !agente.Estado;
        await _agentService.ChangeStatusAsync(id, activar);

        TempData["Mensaje"] = activar
            ? "El agente fue activado correctamente."
            : "El agente fue inactivado correctamente.";

        return RedirectToAction(nameof(Agentes));
    }

    [HttpGet]
    public async Task<IActionResult> EliminarAgente(string id)
    {
        var agente = await _agentService.GetByIdAsync(id);
        if (agente is null)
        {
            TempData["Error"] = "El agente seleccionado no existe.";
            return RedirectToAction(nameof(Agentes));
        }

        return View(_mapper.Map<AgentAdminListItemViewModel>(agente));
    }

    [HttpPost]
    [ActionName("EliminarAgente")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarAgenteConfirmado(string id)
    {
        try
        {
            var eliminado = await _agentService.DeleteAgentAsync(id);
            TempData[eliminado ? "Mensaje" : "Error"] = eliminado
                ? "El agente fue eliminado correctamente."
                : "El agente seleccionado no existe.";
        }
        catch (Exception)
        {
            TempData["Error"] = "No fue posible eliminar el agente. Intente nuevamente más tarde.";
        }

        return RedirectToAction(nameof(Agentes));
    }
}
