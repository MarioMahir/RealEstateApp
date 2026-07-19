namespace RealEstateApp.Core.ViewModels.Property;

public class PropertyDetailViewModel
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public List<string> Imagenes { get; set; } = new();
    public string TipoPropiedad { get; set; } = string.Empty;
    public string TipoVenta { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int CantidadHabitaciones { get; set; }
    public int CantidadBanos { get; set; }
    public decimal TamanoTerreno { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public List<string> Mejoras { get; set; } = new();

    public string AgentId { get; set; } = string.Empty;
    public string NombreAgente { get; set; } = string.Empty;
    public string TelefonoAgente { get; set; } = string.Empty;
    public string? FotoAgente { get; set; }
    public string CorreoAgente { get; set; } = string.Empty;

    public string Estado { get; set; } = string.Empty;
}
