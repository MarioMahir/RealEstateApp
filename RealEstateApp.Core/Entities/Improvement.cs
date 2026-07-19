using RealEstateApp.Core.Interfaces.Entities;

namespace RealEstateApp.Core.Entities;

public class Improvement : ICatalogItem
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;

    public ICollection<PropertyImprovement> PropertyImprovements { get; set; } = new List<PropertyImprovement>();
}
