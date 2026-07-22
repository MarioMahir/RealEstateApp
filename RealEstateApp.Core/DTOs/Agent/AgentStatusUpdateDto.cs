using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.DTOs.Agent;

public class AgentStatusUpdateDto
{
    [Required(ErrorMessage = "El estado enviado no es válido.")]
    public bool? Estado { get; set; }
}
