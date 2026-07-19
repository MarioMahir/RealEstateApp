using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Constants;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Interfaces.Services;
using RealEstateApp.Core.ViewModels.Property;

namespace RealEstateApp.WebApp.Controllers;

// Etapa 2: solo lectura (ver las propias propiedades, Disponibles + Vendidas).
// Etapa 4 agrega alta/edicion/eliminacion ("mantenimiento de propiedades").
[Authorize(Roles = Roles.Agente)]
public class AgenteController : Controller
{
    private readonly IPropertyService _propertyService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public AgenteController(IPropertyService propertyService, UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _propertyService = propertyService;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        var agentId = _userManager.GetUserId(User)!;
        var propiedades = await _propertyService.GetAllByAgentAsync(agentId);

        var modelo = _mapper.Map<List<PropertyListItemViewModel>>(propiedades);
        if (modelo.Count == 0)
            ViewData["Mensaje"] = "Todavía no tiene propiedades registradas.";

        return View(modelo);
    }
}
