using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Models;

namespace RealEstateApp.Core.Interfaces.Services;

public interface IOfferService : IGenericService<Offer>
{
    // Acepta la oferta indicada, rechaza automaticamente todas las demas ofertas
    // Pendiente de la misma propiedad, y marca la propiedad como Vendida -- todo
    // en una sola transaccion (ver OfferService para el detalle).
    Task AcceptOfferAsync(int offerId);

    Task RejectOfferAsync(int offerId);

    // Valida las reglas de negocio (propiedad Disponible, sin oferta Pendiente
    // duplicada del mismo cliente) antes de crear la oferta en estado Pendiente.
    Task<OfferCreationResult> CreateOfferAsync(string clienteId, int propertyId, decimal monto);

    // Historial propio del cliente sobre una propiedad puntual (nunca se borra).
    Task<List<Offer>> GetByClienteAndPropertyAsync(string clienteId, int propertyId);
}
