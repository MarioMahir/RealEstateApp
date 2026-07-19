using RealEstateApp.Core.ViewModels.Cliente;
using RealEstateApp.Core.ViewModels.Property;

namespace RealEstateApp.Core.ViewModels.Agente;

// Detalle de propiedad (agente) = informacion completa de la propiedad (misma
// forma que el detalle publico/cliente, reusada aqui) + conversaciones
// agrupadas por cliente (listado -> hilo completo + responder) + ofertas
// agrupadas por cliente (listado -> aceptar/rechazar).
public class AgentPropertyDetailViewModel
{
    public PropertyDetailViewModel Propiedad { get; set; } = new();

    public List<ConversationSummaryViewModel> Conversaciones { get; set; } = new();
    public List<OfferGroupViewModel> GruposDeOfertas { get; set; } = new();

    // Cliente cuyo hilo completo esta expandido (null = ninguno seleccionado).
    public string? ClienteSeleccionadoId { get; set; }
    public string? ClienteSeleccionadoNombre { get; set; }
    public List<MessageItemViewModel> HiloSeleccionado { get; set; } = new();

    public AgentReplyViewModel Respuesta { get; set; } = new();
}
