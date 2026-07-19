using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.ViewModels.Account;

public class LoginViewModel
{
    [Required(ErrorMessage = "Debe ingresar su correo o nombre de usuario y contraseña.")]
    [Display(Name = "Correo o nombre de usuario")]
    public string UsuarioOCorreo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe ingresar su correo o nombre de usuario y contraseña.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Contrasena { get; set; } = string.Empty;
}
