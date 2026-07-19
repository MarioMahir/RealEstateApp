using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.ViewModels.Cliente;

public class SendMessageViewModel
{
    [Required]
    public int PropertyId { get; set; }

    [Required(ErrorMessage = "Debe escribir un mensaje.")]
    [StringLength(1000, ErrorMessage = "El mensaje no puede superar los 1000 caracteres.")]
    [Display(Name = "Mensaje")]
    public string Texto { get; set; } = string.Empty;
}
