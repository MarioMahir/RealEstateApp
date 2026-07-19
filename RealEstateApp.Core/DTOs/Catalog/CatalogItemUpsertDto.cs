using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.DTOs.Catalog;

// Cuerpo de solicitud compartido por Create/Update de PropertyType, SaleType e
// Improvement: los tres piden exactamente Nombre + Descripcion.
public class CatalogItemUpsertDto
{
    [Required(ErrorMessage = "El nombre es requerido.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es requerida.")]
    public string Descripcion { get; set; } = string.Empty;
}
