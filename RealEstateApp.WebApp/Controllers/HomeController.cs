using System.Diagnostics;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Constants;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Interfaces.Services;
using RealEstateApp.Core.ViewModels.Property;
using RealEstateApp.WebApp.Helpers;
using RealEstateApp.WebApp.Models;

namespace RealEstateApp.WebApp.Controllers;

public class HomeController : Controller
{
    private readonly IPropertyService _propertyService;
    private readonly IGenericService<PropertyType> _propertyTypeService;
    private readonly IFavoriteService _favoriteService;
    private readonly IMapper _mapper;

    public HomeController(
        IPropertyService propertyService,
        IGenericService<PropertyType> propertyTypeService,
        IFavoriteService favoriteService,
        IMapper mapper)
    {
        _propertyService = propertyService;
        _propertyTypeService = propertyTypeService;
        _favoriteService = favoriteService;
        _mapper = mapper;
    }

    // Publico: cualquier visitante (autenticado o no) puede buscar/filtrar
    // propiedades disponibles. Mismos filtros que Agentes/Propiedades. Para un
    // Cliente autenticado esta MISMA accion es "Home del cliente" (= Home
    // publico + marcar/desmarcar favorito), sin una ruta separada.
    [HttpGet]
    public async Task<IActionResult> Index(PropertyBrowseViewModel filtro)
    {
        filtro.TiposPropiedad = await PropertyBrowseHelper.GetTiposPropiedadAsync(_propertyTypeService);

        if (!ModelState.IsValid)
            return View(filtro);

        await PropertyBrowseHelper.ApplyAsync(filtro, _propertyService, _mapper);

        if (User.IsInRole(Roles.Cliente))
        {
            var clienteId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var favoritos = await _favoriteService.GetFavoritePropertyIdsAsync(clienteId);
            foreach (var item in filtro.Propiedades)
                item.EsFavorito = favoritos.Contains(item.Id);

            ViewData["ContextoCliente"] = true;
        }

        return View(filtro);
    }

    public async Task<IActionResult> Details(int id)
    {
        // El Cliente tiene su propia version enriquecida (chat + ofertas) en
        // ClienteController.Detalle; esta vista publica nunca debe mostrarsela.
        if (User.IsInRole(Roles.Cliente))
            return RedirectToAction(nameof(ClienteController.Detalle), "Cliente", new { id });

        var propiedad = await _propertyService.GetAvailableByIdAsync(id);
        if (propiedad is null)
        {
            TempData["Error"] = "La propiedad solicitada no existe o no se encuentra disponible.";
            return RedirectToAction(nameof(Index));
        }

        return View(_mapper.Map<PropertyDetailViewModel>(propiedad));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
