using AutoMapper;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Interfaces.Services;

namespace RealEstateApp.WebAPI.Controllers;

public class SaleTypesController : CatalogControllerBase<SaleType>
{
    public SaleTypesController(IGenericService<SaleType> service, IMapper mapper) : base(service, mapper)
    {
    }

    protected override string NotFoundMessage => "El tipo de venta solicitado no existe.";
    protected override string DuplicateNameMessage => "Ya existe un tipo de venta registrado con este nombre.";
    protected override string DuplicateNameOnUpdateMessage => "Ya existe otro tipo de venta registrado con este nombre.";
}
