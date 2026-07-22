namespace RealEstateApp.Core.ViewModels.Property;

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

    public string Estado { get; set; } = string.Empty;

    public bool EsFavorito { get; set; }
}
