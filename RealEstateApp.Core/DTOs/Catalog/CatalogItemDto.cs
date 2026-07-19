namespace RealEstateApp.Core.DTOs.Catalog;

// Forma de respuesta compartida por PropertyType, SaleType e Improvement.
public class CatalogItemDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}
