using RealEstateApp.Core.ViewModels.Property;

namespace RealEstateApp.Core.ViewModels.Cliente;

public class ClientPropertyDetailViewModel
{
    public PropertyDetailViewModel Propiedad { get; set; } = new();

    public bool PermiteNuevaOferta { get; set; }

    public List<OfferItemViewModel> MisOfertas { get; set; } = new();
    public List<MessageItemViewModel> Mensajes { get; set; } = new();

    public SendMessageViewModel NuevoMensaje { get; set; } = new();
    public CreateOfferViewModel NuevaOferta { get; set; } = new();
}
