using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Constants;
using RealEstateApp.Core.Interfaces.Entities;
using RealEstateApp.Core.Interfaces.Services;
using RealEstateApp.Core.ViewModels.Administrador;

namespace RealEstateApp.WebApp.Controllers;

// Base compartida por los 3 mantenimientos de catalogo (tipo de propiedad,
// tipo de venta, mejora): mismo CRUD exacto, solo cambian los mensajes y como
// se cuentan las propiedades asociadas (ver ObtenerConteosAsync). Mismo
// patron que WebAPI/Controllers/CatalogControllerBase<TEntity> (Etapa 1),
// aplicado ahora a la WebApp con vistas compartidas en Views/Shared/Catalogo/.
[Authorize(Roles = Roles.Administrador)]
public abstract class CatalogMaintenanceControllerBase<TEntity> : Controller where TEntity : class, ICatalogItem, new()
{
    protected readonly IGenericService<TEntity> Service;
    private readonly IFileStorageService _fileStorageService;

    protected CatalogMaintenanceControllerBase(IGenericService<TEntity> service, IFileStorageService fileStorageService)
    {
        Service = service;
        _fileStorageService = fileStorageService;
    }

    protected abstract string Titulo { get; }
    protected abstract string TituloNuevo { get; }
    protected abstract string TituloEditar { get; }
    protected abstract string EmptyListMessage { get; }
    protected abstract string NotFoundMessage { get; }
    protected abstract string DuplicateNameMessage { get; }
    protected abstract string DuplicateNameOnUpdateMessage { get; }
    protected abstract string CreateSuccessMessage { get; }
    protected abstract string UpdateSuccessMessage { get; }
    protected abstract string DeleteConfirmMessage { get; }
    protected abstract string DeleteErrorMessage { get; }
    protected abstract string DeleteSuccessMessage { get; }

    // Cada catalogo cuenta "propiedades asociadas" de forma distinta
    // (PropertyTypeId/SaleTypeId son FK directo; Improvement es N:M via
    // PropertyImprovement), asi que esto lo resuelve cada subclase concreta.
    protected abstract Task<Dictionary<int, int>> ObtenerConteosAsync();

    // URLs de imagenes que quedaran huerfanas en disco si se elimina este item
    // (cascada real de FK hacia Property) -- vacio por defecto, ya que
    // eliminar una Mejora nunca cascada Property/imagenes. PropertyType y
    // SaleType lo sobrescriben porque su FK hacia Property si es Cascade.
    protected virtual Task<List<string>> ObtenerImagenesAEliminarAsync(int id) => Task.FromResult(new List<string>());

    public async Task<IActionResult> Index()
    {
        CargarViewData();

        var items = await Service.GetAllAsync();
        var conteos = await ObtenerConteosAsync();

        var modelo = items
            .OrderBy(i => i.Nombre)
            .Select(i => new CatalogItemViewModel
            {
                Id = i.Id,
                Nombre = i.Nombre,
                Descripcion = i.Descripcion,
                CantidadPropiedades = conteos.GetValueOrDefault(i.Id)
            })
            .ToList();

        return View("~/Views/Shared/Catalogo/Index.cshtml", modelo);
    }

    [HttpGet]
    public IActionResult Crear()
    {
        CargarViewData();
        return View("~/Views/Shared/Catalogo/Form.cshtml", new CatalogItemViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CatalogItemViewModel modelo)
    {
        CargarViewData();

        if (!ModelState.IsValid)
            return View("~/Views/Shared/Catalogo/Form.cshtml", modelo);

        var nombre = modelo.Nombre.Trim();
        var existentes = await Service.GetAllAsync();
        if (existentes.Any(x => x.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError(nameof(modelo.Nombre), DuplicateNameMessage);
            return View("~/Views/Shared/Catalogo/Form.cshtml", modelo);
        }

        var entidad = new TEntity { Nombre = nombre, Descripcion = modelo.Descripcion };
        await Service.AddAsync(entidad);

        TempData["Mensaje"] = CreateSuccessMessage;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        CargarViewData();

        var entidad = await Service.GetByIdAsync(id);
        if (entidad is null)
        {
            TempData["Error"] = NotFoundMessage;
            return RedirectToAction(nameof(Index));
        }

        return View("~/Views/Shared/Catalogo/Form.cshtml",
            new CatalogItemViewModel { Id = entidad.Id, Nombre = entidad.Nombre, Descripcion = entidad.Descripcion });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, CatalogItemViewModel modelo)
    {
        CargarViewData();

        if (!ModelState.IsValid)
        {
            modelo.Id = id;
            return View("~/Views/Shared/Catalogo/Form.cshtml", modelo);
        }

        var entidad = await Service.GetByIdAsync(id);
        if (entidad is null)
        {
            TempData["Error"] = NotFoundMessage;
            return RedirectToAction(nameof(Index));
        }

        var nombre = modelo.Nombre.Trim();
        var existentes = await Service.GetAllAsync();
        if (existentes.Any(x => x.Id != id && x.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError(nameof(modelo.Nombre), DuplicateNameOnUpdateMessage);
            modelo.Id = id;
            return View("~/Views/Shared/Catalogo/Form.cshtml", modelo);
        }

        entidad.Nombre = nombre;
        entidad.Descripcion = modelo.Descripcion;
        await Service.UpdateAsync(entidad);

        TempData["Mensaje"] = UpdateSuccessMessage;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Eliminar(int id)
    {
        CargarViewData();

        var entidad = await Service.GetByIdAsync(id);
        if (entidad is null)
        {
            TempData["Error"] = NotFoundMessage;
            return RedirectToAction(nameof(Index));
        }

        ViewData["DeleteConfirmMessage"] = DeleteConfirmMessage;
        return View("~/Views/Shared/Catalogo/Eliminar.cshtml",
            new CatalogItemViewModel { Id = entidad.Id, Nombre = entidad.Nombre, Descripcion = entidad.Descripcion });
    }

    [HttpPost]
    [ActionName("Eliminar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarConfirmado(int id)
    {
        var entidad = await Service.GetByIdAsync(id);
        if (entidad is null)
        {
            TempData["Error"] = NotFoundMessage;
            return RedirectToAction(nameof(Index));
        }

        var imagenesAEliminar = await ObtenerImagenesAEliminarAsync(id);
        try
        {
            await Service.DeleteAsync(entidad);
            foreach (var url in imagenesAEliminar)
                _fileStorageService.DeleteImage(url);
            TempData["Mensaje"] = DeleteSuccessMessage;
        }
        catch (Exception)
        {
            TempData["Error"] = DeleteErrorMessage;
        }

        return RedirectToAction(nameof(Index));
    }

    private void CargarViewData()
    {
        ViewData["Titulo"] = Titulo;
        ViewData["TituloNuevo"] = TituloNuevo;
        ViewData["TituloEditar"] = TituloEditar;
        ViewData["EmptyListMessage"] = EmptyListMessage;
    }
}
