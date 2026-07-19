namespace RealEstateApp.Core.Models;

// Los mismos filtros deben aplicar en toda pantalla que liste propiedades
// disponibles (Home publico, Home del cliente, propiedades de un agente).
public class PropertyFilterCriteria
{
    public int? PropertyTypeId { get; set; }
    public decimal? PrecioMinimo { get; set; }
    public decimal? PrecioMaximo { get; set; }
    public int? CantidadHabitaciones { get; set; }
    public int? CantidadBanos { get; set; }
}
