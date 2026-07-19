using AutoMapper;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Interfaces.Services;

namespace RealEstateApp.WebAPI.Controllers;

public class ImprovementsController : CatalogControllerBase<Improvement>
{
    public ImprovementsController(IGenericService<Improvement> service, IMapper mapper) : base(service, mapper)
    {
    }

    protected override string NotFoundMessage => "La mejora solicitada no existe.";
    protected override string DuplicateNameMessage => "Ya existe una mejora registrada con este nombre.";
    protected override string DuplicateNameOnUpdateMessage => "Ya existe otra mejora registrada con este nombre.";
}
