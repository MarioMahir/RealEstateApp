using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.ViewModels.Agente;

public class AgentReplyViewModel
{
    [Required]
    public int PropertyId { get; set; }

    [Required]
    public string ClienteId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe escribir un mensaje antes de enviarlo.")]
    [StringLength(1000, ErrorMessage = "El mensaje no puede superar los 1000 caracteres.")]
    [Display(Name = "Mensaje")]
    public string Texto { get; set; } = string.Empty;
}
