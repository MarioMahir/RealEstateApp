using RealEstateApp.Core.Entities;

namespace RealEstateApp.Core.Interfaces.Services;

public interface IOfferService : IGenericService<Offer>
{
    // Acepta la oferta indicada, rechaza automaticamente todas las demas ofertas
    // Pendiente de la misma propiedad, y marca la propiedad como Vendida -- todo
    // en una sola transaccion (ver OfferService para el detalle).
    Task AcceptOfferAsync(int offerId);

    Task RejectOfferAsync(int offerId);
}
