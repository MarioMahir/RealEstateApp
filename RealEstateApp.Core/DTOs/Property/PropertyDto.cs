namespace RealEstateApp.Core.DTOs.Property;

public class PropertyDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string TipoPropiedad { get; set; } = string.Empty;
    public string TipoVenta { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public decimal TamanoTerreno { get; set; }
    public int CantidadHabitaciones { get; set; }
    public int CantidadBanos { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public List<string> Mejoras { get; set; } = new();
    public string NombreAgente { get; set; } = string.Empty;
    public string IdAgente { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}
