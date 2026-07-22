namespace RealEstateApp.Core.Interfaces.Entities;

public interface ICatalogItem
{
    int Id { get; set; }
    string Nombre { get; set; }
    string Descripcion { get; set; }
}
