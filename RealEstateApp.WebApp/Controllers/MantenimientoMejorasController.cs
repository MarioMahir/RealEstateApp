using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Interfaces.Services;

namespace RealEstateApp.WebApp.Controllers;

public class MantenimientoMejorasController : CatalogMaintenanceControllerBase<Improvement>
{
    private readonly IPropertyService _propertyService;

    public MantenimientoMejorasController(
        IGenericService<Improvement> service, IPropertyService propertyService, IFileStorageService fileStorageService)
        : base(service, fileStorageService)
    {
        _propertyService = propertyService;
    }

    protected override string Titulo => "Mantenimiento de mejoras";
    protected override string TituloNuevo => "Crear mejora";
    protected override string TituloEditar => "Editar mejora";
    protected override string EmptyListMessage => "No existen mejoras registradas.";
    protected override string NotFoundMessage => "La mejora seleccionada no existe.";
    protected override string DuplicateNameMessage => "Ya existe una mejora registrada con este nombre.";
    protected override string DuplicateNameOnUpdateMessage => "Ya existe otra mejora registrada con este nombre.";
    protected override string CreateSuccessMessage => "La mejora fue creada correctamente.";
    protected override string UpdateSuccessMessage => "La mejora fue actualizada correctamente.";
    protected override string DeleteConfirmMessage => "¿Está seguro que desea eliminar esta mejora?";
    protected override string DeleteErrorMessage => "No fue posible eliminar la mejora. Intente nuevamente más tarde.";
    protected override string DeleteSuccessMessage => "La mejora fue eliminada correctamente.";

    protected override async Task<Dictionary<int, int>> ObtenerConteosAsync()
    {
        var propiedades = await _propertyService.GetAllWithDetailsAsync();
        return propiedades
            .SelectMany(p => p.PropertyImprovements.Select(pi => pi.ImprovementId))
            .GroupBy(id => id)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}
