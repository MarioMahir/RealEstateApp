using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Interfaces.Services;

namespace RealEstateApp.WebApp.Controllers;

public class MantenimientoTipoPropiedadesController : CatalogMaintenanceControllerBase<PropertyType>
{
    private readonly IPropertyService _propertyService;

    public MantenimientoTipoPropiedadesController(IGenericService<PropertyType> service, IPropertyService propertyService)
        : base(service)
    {
        _propertyService = propertyService;
    }

    protected override string Titulo => "Mantenimiento de tipo de propiedades";
    protected override string TituloNuevo => "Crear tipo de propiedad";
    protected override string TituloEditar => "Editar tipo de propiedad";
    protected override string EmptyListMessage => "No existen tipos de propiedades registrados.";
    protected override string NotFoundMessage => "El tipo de propiedad seleccionado no existe.";
    protected override string DuplicateNameMessage => "Ya existe un tipo de propiedad registrado con este nombre.";
    protected override string DuplicateNameOnUpdateMessage => "Ya existe otro tipo de propiedad registrado con este nombre.";
    protected override string CreateSuccessMessage => "El tipo de propiedad fue creado correctamente.";
    protected override string UpdateSuccessMessage => "El tipo de propiedad fue actualizado correctamente.";
    protected override string DeleteConfirmMessage =>
        "¿Está seguro que desea eliminar este tipo de propiedad y todas las propiedades asociadas?";
    protected override string DeleteErrorMessage => "No fue posible eliminar el tipo de propiedad. Intente nuevamente más tarde.";
    protected override string DeleteSuccessMessage => "El tipo de propiedad fue eliminado correctamente.";

    protected override async Task<Dictionary<int, int>> ObtenerConteosAsync()
    {
        var propiedades = await _propertyService.GetAllWithDetailsAsync();
        return propiedades.GroupBy(p => p.PropertyTypeId).ToDictionary(g => g.Key, g => g.Count());
    }
}
