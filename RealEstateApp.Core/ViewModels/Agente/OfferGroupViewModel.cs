using RealEstateApp.Core.ViewModels.Cliente;

namespace RealEstateApp.Core.ViewModels.Agente;

public class OfferGroupViewModel
{
    public string ClienteId { get; set; } = string.Empty;
    public string NombreCliente { get; set; } = string.Empty;
    public List<OfferItemViewModel> Ofertas { get; set; } = new();
}
