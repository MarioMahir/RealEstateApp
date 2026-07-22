using RealEstateApp.Core.ViewModels.Cliente;
using RealEstateApp.Core.ViewModels.Property;

namespace RealEstateApp.Core.ViewModels.Agente;

public class AgentPropertyDetailViewModel
{
    public PropertyDetailViewModel Propiedad { get; set; } = new();

    public List<ConversationSummaryViewModel> Conversaciones { get; set; } = new();
    public List<OfferGroupViewModel> GruposDeOfertas { get; set; } = new();

    public string? ClienteSeleccionadoId { get; set; }
    public string? ClienteSeleccionadoNombre { get; set; }
    public List<MessageItemViewModel> HiloSeleccionado { get; set; } = new();

    public AgentReplyViewModel Respuesta { get; set; } = new();
}
