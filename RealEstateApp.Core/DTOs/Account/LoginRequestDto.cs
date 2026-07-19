using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.DTOs.Account;

public class LoginRequestDto
{
    [Required(ErrorMessage = "El usuario o correo electrónico es requerido.")]
    public string UsuarioOCorreo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida.")]
    public string Contrasena { get; set; } = string.Empty;
}
