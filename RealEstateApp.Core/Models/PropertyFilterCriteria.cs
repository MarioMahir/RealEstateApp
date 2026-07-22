namespace RealEstateApp.Core.Models;

public class PropertyFilterCriteria
{
    public int? PropertyTypeId { get; set; }
    public decimal? PrecioMinimo { get; set; }
    public decimal? PrecioMaximo { get; set; }
    public int? CantidadHabitaciones { get; set; }
    public int? CantidadBanos { get; set; }
}
