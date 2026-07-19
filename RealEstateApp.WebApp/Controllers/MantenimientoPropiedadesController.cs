using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Constants;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Enums;
using RealEstateApp.Core.Interfaces.Services;
using RealEstateApp.Core.ViewModels;
using RealEstateApp.Core.ViewModels.Agente;
using RealEstateApp.Core.ViewModels.Property;

namespace RealEstateApp.WebApp.Controllers;

// "Mantenimiento de propiedades" (Agente): listado solo Disponible + crear +
// editar + eliminar. Una propiedad Vendida ya no se puede editar ni eliminar
// desde aqui (sigue viendose en Home del agente, con etiqueta).
[Authorize(Roles = Roles.Agente)]
public class MantenimientoPropiedadesController : Controller
{
    private readonly IPropertyService _propertyService;
    private readonly IGenericService<PropertyType> _propertyTypeService;
    private readonly IGenericService<SaleType> _saleTypeService;
    private readonly IGenericService<Improvement> _improvementService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IMapper _mapper;

    public MantenimientoPropiedadesController(
        IPropertyService propertyService,
        IGenericService<PropertyType> propertyTypeService,
        IGenericService<SaleType> saleTypeService,
        IGenericService<Improvement> improvementService,
        IFileStorageService fileStorageService,
        IMapper mapper)
    {
        _propertyService = propertyService;
        _propertyTypeService = propertyTypeService;
        _saleTypeService = saleTypeService;
        _improvementService = improvementService;
        _fileStorageService = fileStorageService;
        _mapper = mapper;
    }

    private string AgentId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public async Task<IActionResult> Index()
    {
        var propiedades = await _propertyService.GetAvailableByAgentAsync(AgentId);
        var modelo = _mapper.Map<List<PropertyListItemViewModel>>(propiedades);

        if (modelo.Count == 0)
            ViewData["Mensaje"] = "Todavía no tiene propiedades registradas.";

        return View(modelo);
    }

    [HttpGet]
    public async Task<IActionResult> Crear()
    {
        var modelo = new PropertyFormViewModel();
        await CargarCatalogosAsync(modelo);
        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(PropertyFormViewModel modelo, List<IFormFile> nuevasImagenes)
    {
        if (nuevasImagenes.Count == 0 || nuevasImagenes.Count > 4)
            ModelState.AddModelError(string.Empty, "Debe adjuntar entre 1 y 4 imágenes.");

        if (!ModelState.IsValid)
        {
            await CargarCatalogosAsync(modelo);
            return View(modelo);
        }

        List<string> urls;
        try
        {
            urls = await GuardarImagenesAsync(nuevasImagenes);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await CargarCatalogosAsync(modelo);
            return View(modelo);
        }

        var propiedad = new Property
        {
            AgentId = AgentId,
            PropertyTypeId = modelo.PropertyTypeId!.Value,
            SaleTypeId = modelo.SaleTypeId!.Value,
            Precio = modelo.Precio!.Value,
            TamanoTerreno = modelo.TamanoTerreno!.Value,
            CantidadHabitaciones = modelo.CantidadHabitaciones!.Value,
            CantidadBanos = modelo.CantidadBanos!.Value,
            Descripcion = modelo.Descripcion
        };

        var creada = await _propertyService.CreatePropertyAsync(propiedad, modelo.ImprovementIds, urls);

        TempData["Mensaje"] = $"La propiedad fue creada correctamente con el código {creada.Codigo}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var propiedad = await _propertyService.GetByIdForAgentAsync(id, AgentId);
        if (propiedad is null || propiedad.Estado != PropertyStatus.Disponible)
        {
            TempData["Error"] = "La propiedad solicitada no existe o ya no se puede editar.";
            return RedirectToAction(nameof(Index));
        }

        var modelo = new PropertyFormViewModel
        {
            Id = propiedad.Id,
            Codigo = propiedad.Codigo,
            PropertyTypeId = propiedad.PropertyTypeId,
            SaleTypeId = propiedad.SaleTypeId,
            Precio = propiedad.Precio,
            TamanoTerreno = propiedad.TamanoTerreno,
            CantidadHabitaciones = propiedad.CantidadHabitaciones,
            CantidadBanos = propiedad.CantidadBanos,
            Descripcion = propiedad.Descripcion,
            ImprovementIds = propiedad.PropertyImprovements.Select(pi => pi.ImprovementId).ToList(),
            ImagenesActuales = propiedad.Images.Select(i => new ExistingImageViewModel { Id = i.Id, Url = i.Url }).ToList()
        };
        await CargarCatalogosAsync(modelo);
        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, PropertyFormViewModel modelo, List<IFormFile> nuevasImagenes)
    {
        var existente = await _propertyService.GetByIdForAgentAsync(id, AgentId);
        if (existente is null || existente.Estado != PropertyStatus.Disponible)
        {
            TempData["Error"] = "La propiedad solicitada no existe o ya no se puede editar.";
            return RedirectToAction(nameof(Index));
        }

        var totalImagenes = existente.Images.Count - modelo.ImagenesAEliminar.Count + nuevasImagenes.Count;
        if (totalImagenes < 1 || totalImagenes > 4)
            ModelState.AddModelError(string.Empty, "La propiedad debe tener entre 1 y 4 imágenes.");

        if (!ModelState.IsValid)
        {
            await PrepararReedicionAsync(modelo, existente);
            return View(modelo);
        }

        List<string> urls;
        try
        {
            urls = await GuardarImagenesAsync(nuevasImagenes);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await PrepararReedicionAsync(modelo, existente);
            return View(modelo);
        }

        var cambios = new Property
        {
            Id = id,
            AgentId = AgentId,
            PropertyTypeId = modelo.PropertyTypeId!.Value,
            SaleTypeId = modelo.SaleTypeId!.Value,
            Precio = modelo.Precio!.Value,
            TamanoTerreno = modelo.TamanoTerreno!.Value,
            CantidadHabitaciones = modelo.CantidadHabitaciones!.Value,
            CantidadBanos = modelo.CantidadBanos!.Value,
            Descripcion = modelo.Descripcion
        };

        await _propertyService.UpdatePropertyAsync(cambios, modelo.ImprovementIds, modelo.ImagenesAEliminar, urls);

        TempData["Mensaje"] = "La propiedad fue actualizada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Eliminar(int id)
    {
        var propiedad = await _propertyService.GetByIdForAgentAsync(id, AgentId);
        if (propiedad is null || propiedad.Estado != PropertyStatus.Disponible)
        {
            TempData["Error"] = "La propiedad solicitada no existe o ya no se puede eliminar.";
            return RedirectToAction(nameof(Index));
        }

        return View(_mapper.Map<PropertyListItemViewModel>(propiedad));
    }

    [HttpPost]
    [ActionName("Eliminar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarConfirmado(int id)
    {
        var eliminada = await _propertyService.DeletePropertyAsync(id, AgentId);
        TempData[eliminada ? "Mensaje" : "Error"] = eliminada
            ? "La propiedad fue eliminada correctamente."
            : "La propiedad solicitada no existe o ya no se puede eliminar.";

        return RedirectToAction(nameof(Index));
    }

    private async Task<List<string>> GuardarImagenesAsync(List<IFormFile> archivos)
    {
        var urls = new List<string>();
        foreach (var archivo in archivos)
            urls.Add(await _fileStorageService.SaveImageAsync(archivo.OpenReadStream(), archivo.FileName, "propiedades"));

        return urls;
    }

    private async Task PrepararReedicionAsync(PropertyFormViewModel modelo, Property existente)
    {
        modelo.Id = existente.Id;
        modelo.Codigo = existente.Codigo;
        modelo.ImagenesActuales = existente.Images.Select(i => new ExistingImageViewModel { Id = i.Id, Url = i.Url }).ToList();
        await CargarCatalogosAsync(modelo);
    }

    private async Task CargarCatalogosAsync(PropertyFormViewModel modelo)
    {
        var tiposPropiedad = await _propertyTypeService.GetAllAsync();
        modelo.TiposPropiedad = tiposPropiedad
            .Select(t => new SelectOption { Value = t.Id.ToString(), Text = t.Nombre })
            .OrderBy(o => o.Text)
            .ToList();

        var tiposVenta = await _saleTypeService.GetAllAsync();
        modelo.TiposVenta = tiposVenta
            .Select(t => new SelectOption { Value = t.Id.ToString(), Text = t.Nombre })
            .OrderBy(o => o.Text)
            .ToList();

        var mejoras = await _improvementService.GetAllAsync();
        modelo.TodasLasMejoras = mejoras
            .Select(m => new SelectOption { Value = m.Id.ToString(), Text = m.Nombre })
            .OrderBy(o => o.Text)
            .ToList();
    }
}
