using RealEstateApp.Core.ViewModels.Property;

namespace RealEstateApp.Core.ViewModels.Cliente;

// Detalle de propiedad (cliente) = Detalle publico + chat con el agente + mis
// ofertas sobre esa propiedad (crear nueva + ver historial propio).
public class ClientPropertyDetailViewModel
{
    public PropertyDetailViewModel Propiedad { get; set; } = new();

    // false si la propiedad ya no esta Disponible o si el cliente ya tiene una
    // oferta Pendiente sobre ella.
    public bool PermiteNuevaOferta { get; set; }

    public List<OfferItemViewModel> MisOfertas { get; set; } = new();
    public List<MessageItemViewModel> Mensajes { get; set; } = new();

    public SendMessageViewModel NuevoMensaje { get; set; } = new();
    public CreateOfferViewModel NuevaOferta { get; set; } = new();
}
