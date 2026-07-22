using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Models;

namespace RealEstateApp.Core.Interfaces.Services;

public interface IOfferService : IGenericService<Offer>
{
    Task<OfferActionStatus> AcceptOfferAsync(int offerId, string agentId, int propertyId);

    Task<OfferActionStatus> RejectOfferAsync(int offerId, string agentId, int propertyId);

    Task<OfferCreationResult> CreateOfferAsync(string clienteId, int propertyId, decimal monto);

    Task<List<Offer>> GetByClienteAndPropertyAsync(string clienteId, int propertyId);

    Task<List<Offer>> GetByPropertyAsync(int propertyId);
}
