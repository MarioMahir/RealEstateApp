using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
    private readonly IMapper _mapper;

    public HomeController(
        IPropertyService propertyService,
        IGenericService<PropertyType> propertyTypeService,
        IMapper mapper)
    {
        _propertyService = propertyService;
        _propertyTypeService = propertyTypeService;
        _mapper = mapper;
    }

    // Publico: cualquier visitante (autenticado o no) puede buscar/filtrar
    // propiedades disponibles. Mismos filtros que Agentes/Propiedades.
    [HttpGet]
    public async Task<IActionResult> Index(PropertyBrowseViewModel filtro)
    {
        filtro.TiposPropiedad = await PropertyBrowseHelper.GetTiposPropiedadAsync(_propertyTypeService);

        if (!ModelState.IsValid)
            return View(filtro);

        await PropertyBrowseHelper.ApplyAsync(filtro, _propertyService, _mapper);
        return View(filtro);
    }

    public async Task<IActionResult> Details(int id)
    {
        var propiedad = await _propertyService.GetAvailableByIdAsync(id);
        if (propiedad is null)
        {
            TempData["Error"] = "La propiedad solicitada no existe o ya no se encuentra disponible.";
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
