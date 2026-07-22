using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.ViewModels.Agente;

public class ProfileViewModel
{
    [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
    [Display(Name = "Apellido")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = string.Empty;

    public string? FotoUrl { get; set; }
}
