using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.ViewModels.Administrador;

public class CatalogItemViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = string.Empty;

    public int CantidadPropiedades { get; set; }
}
