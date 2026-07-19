using AutoMapper;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.ViewModels.Agent;
using RealEstateApp.Core.ViewModels.Cliente;
using RealEstateApp.Core.ViewModels.Property;

namespace RealEstateApp.Core.Mappings;

public class WebAppMappingProfile : Profile
{
    public WebAppMappingProfile()
    {
        CreateMap<Property, PropertyListItemViewModel>()
            .ForMember(d => d.TipoPropiedad, o => o.MapFrom(s => s.PropertyType.Nombre))
            .ForMember(d => d.TipoVenta, o => o.MapFrom(s => s.SaleType.Nombre))
            .ForMember(d => d.ImagenPrincipal, o => o.MapFrom(s => s.Images.Select(i => i.Url).FirstOrDefault()))
            .ForMember(d => d.Estado, o => o.MapFrom(s => s.Estado.ToString()));

        CreateMap<Property, PropertyDetailViewModel>()
            .ForMember(d => d.Imagenes, o => o.MapFrom(s => s.Images.Select(i => i.Url)))
            .ForMember(d => d.TipoPropiedad, o => o.MapFrom(s => s.PropertyType.Nombre))
            .ForMember(d => d.TipoVenta, o => o.MapFrom(s => s.SaleType.Nombre))
            .ForMember(d => d.Mejoras, o => o.MapFrom(s => s.PropertyImprovements.Select(pi => pi.Improvement.Nombre)))
            .ForMember(d => d.AgentId, o => o.MapFrom(s => s.AgentId))
            .ForMember(d => d.NombreAgente, o => o.MapFrom(s => s.Agent.Nombre + " " + s.Agent.Apellido))
            .ForMember(d => d.TelefonoAgente, o => o.MapFrom(s => s.Agent.PhoneNumber))
            .ForMember(d => d.FotoAgente, o => o.MapFrom(s => s.Agent.FotoUrl))
            .ForMember(d => d.CorreoAgente, o => o.MapFrom(s => s.Agent.Email))
            .ForMember(d => d.Estado, o => o.MapFrom(s => s.Estado.ToString()));

        CreateMap<ApplicationUser, AgentListItemViewModel>();

        CreateMap<Offer, OfferItemViewModel>()
            .ForMember(d => d.Estado, o => o.MapFrom(s => s.Estado.ToString()));

        CreateMap<Message, MessageItemViewModel>()
            .ForMember(d => d.Remitente, o => o.MapFrom(s => s.Remitente.ToString()));
    }
}
