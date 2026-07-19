using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Interfaces.Services;

namespace RealEstateApp.WebApp.Controllers;

public class MantenimientoTipoVentasController : CatalogMaintenanceControllerBase<SaleType>
{
    private readonly IPropertyService _propertyService;

    public MantenimientoTipoVentasController(
        IGenericService<SaleType> service, IPropertyService propertyService, IFileStorageService fileStorageService)
        : base(service, fileStorageService)
    {
        _propertyService = propertyService;
    }

    protected override string Titulo => "Mantenimiento de tipo de ventas";
    protected override string TituloNuevo => "Crear tipo de venta";
    protected override string TituloEditar => "Editar tipo de venta";
    protected override string EmptyListMessage => "No existen tipos de ventas registrados.";
    protected override string NotFoundMessage => "El tipo de venta seleccionado no existe.";
    protected override string DuplicateNameMessage => "Ya existe un tipo de venta registrado con este nombre.";
    protected override string DuplicateNameOnUpdateMessage => "Ya existe otro tipo de venta registrado con este nombre.";
    protected override string CreateSuccessMessage => "El tipo de venta fue creado correctamente.";
    protected override string UpdateSuccessMessage => "El tipo de venta fue actualizado correctamente.";
    protected override string DeleteConfirmMessage => "¿Está seguro que desea eliminar este tipo de venta y las propiedades asociadas?";
    protected override string DeleteErrorMessage => "No fue posible eliminar el tipo de venta. Intente nuevamente más tarde.";
    protected override string DeleteSuccessMessage => "El tipo de venta fue eliminado correctamente.";

    protected override async Task<Dictionary<int, int>> ObtenerConteosAsync()
    {
        var propiedades = await _propertyService.GetAllWithDetailsAsync();
        return propiedades.GroupBy(p => p.SaleTypeId).ToDictionary(g => g.Key, g => g.Count());
    }

    protected override async Task<List<string>> ObtenerImagenesAEliminarAsync(int id)
    {
        var propiedades = await _propertyService.GetAllWithDetailsAsync();
        return propiedades.Where(p => p.SaleTypeId == id).SelectMany(p => p.Images.Select(i => i.Url)).ToList();
    }
}
