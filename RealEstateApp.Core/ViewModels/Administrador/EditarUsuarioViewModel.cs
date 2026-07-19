using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.ViewModels.Administrador;

// Compartido por "Editar administrador" y "Editar desarrollador". La
// contrasena es opcional aqui (a diferencia de Crear): si se deja vacia se
// mantiene la actual; si se ingresa, la confirmacion es obligatoria y debe
// coincidir (ver Validate).
public class EditarUsuarioViewModel : IValidatableObject
{
    public string Id { get; set; } = string.Empty;

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

    [DataType(DataType.Password)]
    [Display(Name = "Nueva contraseña")]
    public string? NuevaContrasena { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar nueva contraseña")]
    public string? ConfirmarNuevaContrasena { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (NuevaContrasena != ConfirmarNuevaContrasena)
            yield return new ValidationResult(
                "La contraseña y la confirmación de contraseña no coinciden.",
                new[] { nameof(NuevaContrasena), nameof(ConfirmarNuevaContrasena) });
    }
}
