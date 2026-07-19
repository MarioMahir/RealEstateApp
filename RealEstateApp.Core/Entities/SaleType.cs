namespace RealEstateApp.Core.Entities;

public class SaleType
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;

    public ICollection<Property> Properties { get; set; } = new List<Property>();
}
