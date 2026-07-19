using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Constants;
using RealEstateApp.Core.Interfaces.Services;

namespace RealEstateApp.WebApp.Controllers;

// Etapa 2: solo los indicadores (Home del administrador). Etapa 5 agrega el
// listado de agentes y los mantenimientos de administradores/desarrolladores/catalogos.
[Authorize(Roles = Roles.Administrador)]
public class AdministradorController : Controller
{
    private readonly IAdministradorService _administradorService;

    public AdministradorController(IAdministradorService administradorService)
    {
        _administradorService = administradorService;
    }

    public async Task<IActionResult> Index()
    {
        var indicadores = await _administradorService.GetIndicadoresAsync();
        return View(indicadores);
    }
}
