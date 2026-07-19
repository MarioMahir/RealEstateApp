namespace RealEstateApp.Core.Interfaces.Entities;

// Implementada por PropertyType, SaleType e Improvement: las tres tienen
// exactamente la misma forma (Id, Nombre unico, Descripcion) y el mismo
// patron de mantenimiento en la API. Permite un controlador base generico
// en la WebAPI sin triplicar el CRUD.
public interface ICatalogItem
{
    int Id { get; set; }
    string Nombre { get; set; }
    string Descripcion { get; set; }
}
