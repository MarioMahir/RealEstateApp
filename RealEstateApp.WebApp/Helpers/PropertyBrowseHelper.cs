using AutoMapper;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Interfaces.Services;
using RealEstateApp.Core.Models;
using RealEstateApp.Core.ViewModels;
using RealEstateApp.Core.ViewModels.Property;

namespace RealEstateApp.WebApp.Helpers;

// Logica de busqueda/filtrado compartida por Home (publico/cliente) y por
// Agentes/Propiedades (mismos filtros, alcance opcional a un solo agente) --
// una sola implementacion garantiza que ambas pantallas se comporten
// identico, tal como exige el documento funcional.
internal static class PropertyBrowseHelper
{
    public static async Task ApplyAsync(
        PropertyBrowseViewModel modelo,
        IPropertyService propertyService,
        IMapper mapper,
        string? agentIdScope = null)
    {
        if (!string.IsNullOrWhiteSpace(modelo.Codigo))
        {
            var propiedad = await propertyService.GetAvailableByCodeAsync(modelo.Codigo.Trim());
            if (propiedad is not null && agentIdScope is not null && propiedad.AgentId != agentIdScope)
                propiedad = null;

            modelo.Propiedades = propiedad is null
                ? new List<PropertyListItemViewModel>()
                : new List<PropertyListItemViewModel> { mapper.Map<PropertyListItemViewModel>(propiedad) };

            if (propiedad is null)
                modelo.Mensaje = "No se encontró ninguna propiedad disponible con el código ingresado.";
            return;
        }

        var criteria = new PropertyFilterCriteria
        {
            PropertyTypeId = modelo.PropertyTypeId,
            PrecioMinimo = modelo.PrecioMinimo,
            PrecioMaximo = modelo.PrecioMaximo,
            CantidadHabitaciones = modelo.CantidadHabitaciones,
            CantidadBanos = modelo.CantidadBanos
        };
        var tieneFiltros = criteria.PropertyTypeId.HasValue || criteria.PrecioMinimo.HasValue ||
            criteria.PrecioMaximo.HasValue || criteria.CantidadHabitaciones.HasValue || criteria.CantidadBanos.HasValue;

        var propiedades = agentIdScope is null
            ? await propertyService.SearchAvailableAsync(criteria)
            : await propertyService.GetAvailableByAgentAsync(agentIdScope, criteria);

        modelo.Propiedades = mapper.Map<List<PropertyListItemViewModel>>(propiedades);

        if (modelo.Propiedades.Count == 0)
        {
            modelo.Mensaje = tieneFiltros
                ? "No se encontraron propiedades disponibles con los filtros seleccionados."
                : "Todavía no hay propiedades disponibles publicadas.";
        }
    }

    public static async Task<List<SelectOption>> GetTiposPropiedadAsync(IGenericService<PropertyType> propertyTypeService)
    {
        var tipos = await propertyTypeService.GetAllAsync();
        return tipos
            .Select(t => new SelectOption { Value = t.Id.ToString(), Text = t.Nombre })
            .OrderBy(o => o.Text)
            .ToList();
    }
}
