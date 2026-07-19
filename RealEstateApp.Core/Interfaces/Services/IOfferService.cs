using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Models;

namespace RealEstateApp.Core.Interfaces.Services;

public interface IOfferService : IGenericService<Offer>
{
    // Solo el agente dueno de la propiedad puede aceptar/rechazar, y solo si la
    // oferta sigue Pendiente (aceptar/rechazar una oferta ya resuelta antes
    // corrompería el estado -- rechazaria "las demas pendientes" sin haber
    // aceptado nada nuevo). Acepta la oferta indicada, rechaza automaticamente
    // todas las demas ofertas Pendiente de la misma propiedad, y marca la
    // propiedad como Vendida -- todo en una sola transaccion.
    Task<OfferActionStatus> AcceptOfferAsync(int offerId, string agentId);

    Task<OfferActionStatus> RejectOfferAsync(int offerId, string agentId);

    // Valida las reglas de negocio (propiedad Disponible, sin oferta Pendiente
    // duplicada del mismo cliente) antes de crear la oferta en estado Pendiente.
    Task<OfferCreationResult> CreateOfferAsync(string clienteId, int propertyId, decimal monto);

    // Historial propio del cliente sobre una propiedad puntual (nunca se borra).
    Task<List<Offer>> GetByClienteAndPropertyAsync(string clienteId, int propertyId);

    // Todas las ofertas de una propiedad (cualquier cliente) -- Detalle de
    // propiedad (agente), agrupadas por cliente en el controlador/vista.
    Task<List<Offer>> GetByPropertyAsync(int propertyId);
}
