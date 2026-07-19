using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.ViewModels.Administrador;

// Compartido por los 3 mantenimientos de catalogo (tipo de propiedad, tipo de
// venta, mejora): mismo shape exacto (Nombre + Descripcion). CantidadPropiedades
// solo se usa para el listado (no forma parte del POST de Crear/Editar).
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
