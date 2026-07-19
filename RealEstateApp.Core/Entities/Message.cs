using RealEstateApp.Core.Enums;

namespace RealEstateApp.Core.Entities;

// No existe una entidad "Conversacion" separada: una conversacion es, en la
// practica, el conjunto de Message que comparten (ClienteId, AgenteId, PropertyId).
public class Message
{
    public int Id { get; set; }

    public string ClienteId { get; set; } = string.Empty;
    public ApplicationUser Cliente { get; set; } = null!;

    public string AgenteId { get; set; } = string.Empty;
    public ApplicationUser Agente { get; set; } = null!;

    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;

    public MessageSender Remitente { get; set; }
    public string Texto { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
}
