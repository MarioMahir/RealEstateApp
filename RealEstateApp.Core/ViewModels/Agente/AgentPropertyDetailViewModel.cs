using RealEstateApp.Core.ViewModels.Cliente;

namespace RealEstateApp.Core.ViewModels.Agente;

// Detalle de propiedad (agente) = conversaciones agrupadas por cliente
// (listado -> hilo completo + responder) + ofertas agrupadas por cliente
// (listado -> aceptar/rechazar).
public class AgentPropertyDetailViewModel
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string TipoPropiedad { get; set; } = string.Empty;
    public string TipoVenta { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string Estado { get; set; } = string.Empty;

    public List<ConversationSummaryViewModel> Conversaciones { get; set; } = new();
    public List<OfferGroupViewModel> GruposDeOfertas { get; set; } = new();

    // Cliente cuyo hilo completo esta expandido (null = ninguno seleccionado).
    public string? ClienteSeleccionadoId { get; set; }
    public string? ClienteSeleccionadoNombre { get; set; }
    public List<MessageItemViewModel> HiloSeleccionado { get; set; } = new();

    public AgentReplyViewModel Respuesta { get; set; } = new();
}
