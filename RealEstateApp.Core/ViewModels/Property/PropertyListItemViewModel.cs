namespace RealEstateApp.Core.ViewModels.Property;

// Forma compartida por Home publico, Home del cliente, propiedades de un
// agente (publico) y Home del agente -- misma informacion en todas.
public class PropertyListItemViewModel
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string TipoPropiedad { get; set; } = string.Empty;
    public string TipoVenta { get; set; } = string.Empty;
    public string? ImagenPrincipal { get; set; }
    public decimal Precio { get; set; }
    public decimal TamanoTerreno { get; set; }
    public int CantidadHabitaciones { get; set; }
    public int CantidadBanos { get; set; }

    // Solo se usa en el Home del agente (propias, Disponible + Vendida).
    public string Estado { get; set; } = string.Empty;

    // Solo se usa en pantallas del Cliente (Home del cliente, Mis propiedades).
    public bool EsFavorito { get; set; }
}
