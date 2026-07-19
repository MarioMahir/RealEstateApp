using RealEstateApp.Core.ViewModels.Cliente;

namespace RealEstateApp.Core.ViewModels.Agente;

public class OfferGroupViewModel
{
    public string ClienteId { get; set; } = string.Empty;
    public string NombreCliente { get; set; } = string.Empty;

    // Ordenadas de mas reciente a mas antigua (ver AgenteController); las 3
    // propiedades de resumen de abajo se derivan de ese orden.
    public List<OfferItemViewModel> Ofertas { get; set; } = new();

    public int CantidadOfertas => Ofertas.Count;
    public decimal UltimaOferta => Ofertas.Count > 0 ? Ofertas[0].Monto : 0m;
    public string EstadoUltimaOferta => Ofertas.Count > 0 ? Ofertas[0].Estado : string.Empty;
}
