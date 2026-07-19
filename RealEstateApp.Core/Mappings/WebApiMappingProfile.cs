using AutoMapper;
using RealEstateApp.Core.DTOs.Agent;
using RealEstateApp.Core.DTOs.Catalog;
using RealEstateApp.Core.DTOs.Property;
using RealEstateApp.Core.Entities;

namespace RealEstateApp.Core.Mappings;

public class WebApiMappingProfile : Profile
{
    public WebApiMappingProfile()
    {
        CreateMap<Property, PropertyDto>()
            .ForMember(d => d.TipoPropiedad, o => o.MapFrom(s => s.PropertyType.Nombre))
            .ForMember(d => d.TipoVenta, o => o.MapFrom(s => s.SaleType.Nombre))
            .ForMember(d => d.Mejoras, o => o.MapFrom(s => s.PropertyImprovements.Select(pi => pi.Improvement.Nombre)))
            .ForMember(d => d.NombreAgente, o => o.MapFrom(s => s.Agent.Nombre + " " + s.Agent.Apellido))
            .ForMember(d => d.IdAgente, o => o.MapFrom(s => s.AgentId))
            .ForMember(d => d.Estado, o => o.MapFrom(s => s.Estado.ToString()));

        CreateMap<ApplicationUser, AgentDto>()
            .ForMember(d => d.CorreoElectronico, o => o.MapFrom(s => s.Email))
            .ForMember(d => d.Telefono, o => o.MapFrom(s => s.PhoneNumber))
            .ForMember(d => d.CantidadPropiedades, o => o.MapFrom(s => s.Properties.Count))
            .ForMember(d => d.Estado, o => o.MapFrom(s => s.Activo));

        CreateMap<PropertyType, CatalogItemDto>();
        CreateMap<SaleType, CatalogItemDto>();
        CreateMap<Improvement, CatalogItemDto>();

        CreateMap<CatalogItemUpsertDto, PropertyType>();
        CreateMap<CatalogItemUpsertDto, SaleType>();
        CreateMap<CatalogItemUpsertDto, Improvement>();
    }
}
