using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Interfaces.Services;
using RealEstateApp.Core.ViewModels.Agent;
using RealEstateApp.Core.ViewModels.Property;
using RealEstateApp.WebApp.Helpers;

namespace RealEstateApp.WebApp.Controllers;

public class AgentesController : Controller
{
    private readonly IAgentService _agentService;
    private readonly IPropertyService _propertyService;
    private readonly IGenericService<PropertyType> _propertyTypeService;
    private readonly IMapper _mapper;

    public AgentesController(
        IAgentService agentService,
        IPropertyService propertyService,
        IGenericService<PropertyType> propertyTypeService,
        IMapper mapper)
    {
        _agentService = agentService;
        _propertyService = propertyService;
        _propertyTypeService = propertyTypeService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? nombre)
    {
        var agentes = await _agentService.GetActiveAgentsAsync(nombre);
        ViewData["NombreBusqueda"] = nombre;
        return View(_mapper.Map<List<AgentListItemViewModel>>(agentes));
    }

    [HttpGet]
    public async Task<IActionResult> Propiedades(string id, PropertyBrowseViewModel filtro)
    {
        var agente = await _agentService.GetActiveAgentByIdAsync(id);
        if (agente is null)
        {
            TempData["Error"] = "El agente solicitado no existe o no se encuentra disponible.";
            return RedirectToAction(nameof(Index));
        }

        filtro.TiposPropiedad = await PropertyBrowseHelper.GetTiposPropiedadAsync(_propertyTypeService);

        var modelo = new AgentPropertiesViewModel
        {
            AgentId = agente.Id,
            NombreAgente = $"{agente.Nombre} {agente.Apellido}",
            FotoAgente = agente.FotoUrl,
            Propiedades = filtro
        };

        if (!ModelState.IsValid)
            return View(modelo);

        await PropertyBrowseHelper.ApplyAsync(filtro, _propertyService, _mapper, agente.Id);
        return View(modelo);
    }
}
