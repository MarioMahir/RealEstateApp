using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.DTOs.Catalog;

public class CatalogItemUpsertDto
{
    [Required(ErrorMessage = "El nombre es requerido.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es requerida.")]
    public string Descripcion { get; set; } = string.Empty;
}
