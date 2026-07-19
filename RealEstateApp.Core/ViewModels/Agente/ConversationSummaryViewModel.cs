namespace RealEstateApp.Core.ViewModels.Agente;

public class ConversationSummaryViewModel
{
    public string ClienteId { get; set; } = string.Empty;
    public string NombreCliente { get; set; } = string.Empty;
    public DateTime UltimoMensajeFecha { get; set; }
    public string UltimoMensajeTexto { get; set; } = string.Empty;
}
