using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.ViewModels.Administrador;

public class CrearUsuarioViewModel
{
    [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
    [Display(Name = "Apellido")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
    [Display(Name = "Cédula")]
    public string Cedula { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
    [EmailAddress(ErrorMessage = "Debe ingresar un correo electrónico válido.")]
    [Display(Name = "Correo electrónico")]
    public string CorreoElectronico { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
    [Display(Name = "Nombre de usuario")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Contrasena { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Contrasena), ErrorMessage = "La contraseña y la confirmación de contraseña no coinciden.")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmarContrasena { get; set; } = string.Empty;
}
