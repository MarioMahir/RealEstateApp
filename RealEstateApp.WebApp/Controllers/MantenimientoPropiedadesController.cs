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
            ViewData["Mensaje"] = "No tiene propiedades disponibles registradas en este momento.";

        return View(modelo);
    }

    [HttpGet]
    public async Task<IActionResult> Crear()
    {
        if (!(await _propertyTypeService.GetAllAsync()).Any())
        {
            TempData["Error"] = "No existen tipos de propiedades registrados. Debe crear al menos un tipo de propiedad antes de registrar una propiedad.";
            return RedirectToAction(nameof(Index));
        }

        if (!(await _saleTypeService.GetAllAsync()).Any())
        {
            TempData["Error"] = "No existen tipos de ventas registrados. Debe crear al menos un tipo de venta antes de registrar una propiedad.";
            return RedirectToAction(nameof(Index));
        }

        if (!(await _improvementService.GetAllAsync()).Any())
        {
            TempData["Error"] = "No existen mejoras registradas. Debe crear al menos una mejora antes de registrar una propiedad.";
            return RedirectToAction(nameof(Index));
        }

        var modelo = new PropertyFormViewModel();
        await CargarCatalogosAsync(modelo);
        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(PropertyFormViewModel modelo, List<IFormFile> nuevasImagenes)
    {
        if (nuevasImagenes.Count == 0)
            ModelState.AddModelError(string.Empty, "Debe cargar al menos una imagen de la propiedad.");
        else if (nuevasImagenes.Count > 4)
            ModelState.AddModelError(string.Empty, "Solo se permite registrar hasta 4 imágenes por propiedad.");

        if (!ModelState.IsValid)
        {
            await CargarCatalogosAsync(modelo);
            return View(modelo);
        }

        await ValidarReferenciasAsync(modelo);
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

        await _propertyService.CreatePropertyAsync(propiedad, modelo.ImprovementIds, urls);

        TempData["Mensaje"] = "La propiedad fue creada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var propiedad = await _propertyService.GetByIdWithDetailsAsync(id);
        if (propiedad is null)
        {
            TempData["Error"] = "La propiedad solicitada no existe.";
            return RedirectToAction(nameof(Index));
        }

        if (propiedad.AgentId != AgentId)
        {
            TempData["Error"] = "No tiene permisos para modificar esta propiedad.";
            return RedirectToAction(nameof(Index));
        }

        if (propiedad.Estado != PropertyStatus.Disponible)
        {
            TempData["Error"] = "No se puede modificar una propiedad que ya fue vendida.";
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
        var existente = await _propertyService.GetByIdWithDetailsAsync(id);
        if (existente is null)
        {
            TempData["Error"] = "La propiedad solicitada no existe.";
            return RedirectToAction(nameof(Index));
        }

        if (existente.AgentId != AgentId)
        {
            TempData["Error"] = "No tiene permisos para modificar esta propiedad.";
            return RedirectToAction(nameof(Index));
        }

        if (existente.Estado != PropertyStatus.Disponible)
        {
            TempData["Error"] = "No se puede modificar una propiedad que ya fue vendida.";
            return RedirectToAction(nameof(Index));
        }

        var totalImagenes = existente.Images.Count - modelo.ImagenesAEliminar.Count + nuevasImagenes.Count;
        if (totalImagenes < 1)
            ModelState.AddModelError(string.Empty, "Debe cargar al menos una imagen de la propiedad.");
        else if (totalImagenes > 4)
            ModelState.AddModelError(string.Empty, "Solo se permite registrar hasta 4 imágenes por propiedad.");

        if (!ModelState.IsValid)
        {
            await PrepararReedicionAsync(modelo, existente);
            return View(modelo);
        }

        await ValidarReferenciasAsync(modelo);
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
        var propiedad = await _propertyService.GetByIdWithDetailsAsync(id);
        if (propiedad is null)
        {
            TempData["Error"] = "La propiedad solicitada no existe.";
            return RedirectToAction(nameof(Index));
        }

        if (propiedad.AgentId != AgentId)
        {
            TempData["Error"] = "No tiene permisos para eliminar esta propiedad.";
            return RedirectToAction(nameof(Index));
        }

        if (propiedad.Estado != PropertyStatus.Disponible)
        {
            TempData["Error"] = "No se puede eliminar una propiedad que ya fue vendida.";
            return RedirectToAction(nameof(Index));
        }

        return View(_mapper.Map<PropertyListItemViewModel>(propiedad));
    }

    [HttpPost]
    [ActionName("Eliminar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarConfirmado(int id)
    {
        var propiedad = await _propertyService.GetByIdWithDetailsAsync(id);
        if (propiedad is null)
        {
            TempData["Error"] = "La propiedad solicitada no existe.";
            return RedirectToAction(nameof(Index));
        }

        if (propiedad.AgentId != AgentId)
        {
            TempData["Error"] = "No tiene permisos para eliminar esta propiedad.";
            return RedirectToAction(nameof(Index));
        }

        if (propiedad.Estado != PropertyStatus.Disponible)
        {
            TempData["Error"] = "No se puede eliminar una propiedad que ya fue vendida.";
            return RedirectToAction(nameof(Index));
        }

        await _propertyService.DeletePropertyAsync(id, AgentId);
        TempData["Mensaje"] = "La propiedad fue eliminada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // Defensa contra una condicion de carrera real (no solo teorica): el
    // formulario se llena con el catalogo vigente al momento de cargarlo, pero
    // un Administrador podria eliminar un tipo de propiedad/venta/mejora
    // mientras el agente todavia tiene el formulario abierto. Sin esto, el
    // INSERT/UPDATE fallaria con una FK violation no manejada (500) en vez de
    // un mensaje de validacion normal.
    private async Task ValidarReferenciasAsync(PropertyFormViewModel modelo)
    {
        var tiposValidos = (await _propertyTypeService.GetAllAsync()).Select(t => t.Id).ToHashSet();
        if (modelo.PropertyTypeId.HasValue && !tiposValidos.Contains(modelo.PropertyTypeId.Value))
            ModelState.AddModelError(nameof(modelo.PropertyTypeId), "El tipo de propiedad seleccionado ya no existe.");

        var ventasValidas = (await _saleTypeService.GetAllAsync()).Select(t => t.Id).ToHashSet();
        if (modelo.SaleTypeId.HasValue && !ventasValidas.Contains(modelo.SaleTypeId.Value))
            ModelState.AddModelError(nameof(modelo.SaleTypeId), "El tipo de venta seleccionado ya no existe.");

        var mejorasValidas = (await _improvementService.GetAllAsync()).Select(m => m.Id).ToHashSet();
        if (modelo.ImprovementIds.Any(id => !mejorasValidas.Contains(id)))
            ModelState.AddModelError(string.Empty, "Una o más mejoras seleccionadas ya no existen. Vuelva a intentarlo.");
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
