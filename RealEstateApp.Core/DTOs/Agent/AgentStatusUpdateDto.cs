using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.DTOs.Agent;

public class AgentStatusUpdateDto
{
    // Nullable a proposito: en un bool no-nullable, [Required] es un no-op
    // (un bool siempre "tiene valor", incluso cuando el campo viene ausente
    // del JSON -- el binder simplemente lo deja en el default false, y la
    // validacion nunca detecta nada raro). Con bool? sí se puede distinguir
    // "no vino" de "vino en false".
    [Required(ErrorMessage = "El estado enviado no es válido.")]
    public bool? Estado { get; set; }
}
