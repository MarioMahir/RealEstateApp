using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.DTOs.Account;

// Base compartida por el registro de Administrador y de Desarrollador via API:
// mismos campos exactos segun el documento funcional.
public abstract class RegisterUserDto
{
    [Required(ErrorMessage = "El nombre es requerido.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es requerido.")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "La cédula es requerida.")]
    public string Cedula { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es requerido.")]
    [EmailAddress(ErrorMessage = "Debe ingresar un correo electrónico válido.")]
    public string CorreoElectronico { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre de usuario es requerido.")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida.")]
    public string Contrasena { get; set; } = string.Empty;

    [Required(ErrorMessage = "La confirmación de contraseña es requerida.")]
    [Compare(nameof(Contrasena), ErrorMessage = "La contraseña y la confirmación de contraseña no coinciden.")]
    public string ConfirmarContrasena { get; set; } = string.Empty;
}

public class RegisterAdminDto : RegisterUserDto
{
}

public class RegisterDeveloperDto : RegisterUserDto
{
}
