using AutoMapper;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Interfaces.Services;

namespace RealEstateApp.WebAPI.Controllers;

public class PropertyTypesController : CatalogControllerBase<PropertyType>
{
    public PropertyTypesController(IGenericService<PropertyType> service, IMapper mapper) : base(service, mapper)
    {
    }

    protected override string NotFoundMessage => "El tipo de propiedad solicitado no existe.";
    protected override string DuplicateNameMessage => "Ya existe un tipo de propiedad registrado con este nombre.";
    protected override string DuplicateNameOnUpdateMessage => "Ya existe otro tipo de propiedad registrado con este nombre.";
}
