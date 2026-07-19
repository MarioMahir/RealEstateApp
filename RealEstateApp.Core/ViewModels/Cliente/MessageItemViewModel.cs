namespace RealEstateApp.Core.ViewModels.Cliente;

public class MessageItemViewModel
{
    // "Cliente" o "Agente".
    public string Remitente { get; set; } = string.Empty;
    public string Texto { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
}
