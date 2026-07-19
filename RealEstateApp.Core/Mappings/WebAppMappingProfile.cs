using AutoMapper;
using RealEstateApp.Core.DTOs.Agent;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.ViewModels.Administrador;
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

        CreateMap<AgentDto, AgentAdminListItemViewModel>()
            .ForMember(d => d.Correo, o => o.MapFrom(s => s.CorreoElectronico))
            .ForMember(d => d.Activo, o => o.MapFrom(s => s.Estado));

        CreateMap<ApplicationUser, UsuarioListItemViewModel>()
            .ForMember(d => d.NombreUsuario, o => o.MapFrom(s => s.UserName))
            .ForMember(d => d.CorreoElectronico, o => o.MapFrom(s => s.Email))
            .ForMember(d => d.Cedula, o => o.MapFrom(s => s.Cedula ?? string.Empty));

        CreateMap<PropertyType, CatalogItemViewModel>();
        CreateMap<SaleType, CatalogItemViewModel>();
        CreateMap<Improvement, CatalogItemViewModel>();
    }
}
