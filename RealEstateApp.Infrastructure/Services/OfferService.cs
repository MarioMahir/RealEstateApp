using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Enums;
using RealEstateApp.Core.Interfaces.Repositories;
using RealEstateApp.Core.Interfaces.Services;
using RealEstateApp.Core.Models;

namespace RealEstateApp.Infrastructure.Services;

public class OfferService : GenericService<Offer>, IOfferService
{
    private readonly IGenericRepository<Property> _propertyRepository;

    public OfferService(
        IGenericRepository<Offer> offerRepository,
        IGenericRepository<Property> propertyRepository,
        IUnitOfWork unitOfWork)
        : base(offerRepository, unitOfWork)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<OfferActionStatus> AcceptOfferAsync(int offerId, string agentId)
    {
        var offer = await Repository.Query().Include(o => o.Property).FirstOrDefaultAsync(o => o.Id == offerId);
        if (offer is null) return OfferActionStatus.NotFound;
        if (offer.Property.AgentId != agentId) return OfferActionStatus.NotOwnedByAgent;
        if (offer.Estado != OfferStatus.Pendiente) return OfferActionStatus.NotPending;

        var pendientes = Repository.Query()
            .Where(o => o.PropertyId == offer.PropertyId && o.Estado == OfferStatus.Pendiente)
            .ToList();

        foreach (var pendiente in pendientes)
        {
            pendiente.Estado = pendiente.Id == offer.Id ? OfferStatus.Aceptada : OfferStatus.Rechazada;
            Repository.Update(pendiente);
        }

        offer.Property.Estado = PropertyStatus.Vendida;
        _propertyRepository.Update(offer.Property);

        // Un solo SaveChanges: EF Core lo envuelve en una transaccion implicita,
        // por lo que la oferta aceptada, las rechazadas y el cambio de estado de
        // la propiedad se confirman (o fallan) todos juntos.
        await UnitOfWork.SaveChangesAsync();
        return OfferActionStatus.Success;
    }

    public async Task<OfferActionStatus> RejectOfferAsync(int offerId, string agentId)
    {
        var offer = await Repository.Query().Include(o => o.Property).FirstOrDefaultAsync(o => o.Id == offerId);
        if (offer is null) return OfferActionStatus.NotFound;
        if (offer.Property.AgentId != agentId) return OfferActionStatus.NotOwnedByAgent;
        if (offer.Estado != OfferStatus.Pendiente) return OfferActionStatus.NotPending;

        offer.Estado = OfferStatus.Rechazada;
        Repository.Update(offer);
        await UnitOfWork.SaveChangesAsync();
        return OfferActionStatus.Success;
    }

    public async Task<OfferCreationResult> CreateOfferAsync(string clienteId, int propertyId, decimal monto)
    {
        var property = await _propertyRepository.GetByIdAsync(propertyId);
        if (property is null || property.Estado != PropertyStatus.Disponible)
            return new OfferCreationResult { Status = OfferCreationStatus.PropertyNotAvailable };

        var tienePendiente = Repository.Query()
            .Any(o => o.ClienteId == clienteId && o.PropertyId == propertyId && o.Estado == OfferStatus.Pendiente);
        if (tienePendiente)
            return new OfferCreationResult { Status = OfferCreationStatus.DuplicatePending };

        var offer = new Offer
        {
            ClienteId = clienteId,
            PropertyId = propertyId,
            Monto = monto,
            Fecha = DateTime.Now,
            Estado = OfferStatus.Pendiente
        };

        await Repository.AddAsync(offer);
        await UnitOfWork.SaveChangesAsync();

        return new OfferCreationResult { Status = OfferCreationStatus.Success, Offer = offer };
    }

    public Task<List<Offer>> GetByClienteAndPropertyAsync(string clienteId, int propertyId) =>
        Repository.Query()
            .Where(o => o.ClienteId == clienteId && o.PropertyId == propertyId)
            .OrderByDescending(o => o.Fecha)
            .ToListAsync();

    public Task<List<Offer>> GetByPropertyAsync(int propertyId) =>
        Repository.Query()
            .Where(o => o.PropertyId == propertyId)
            .Include(o => o.Cliente)
            .OrderByDescending(o => o.Fecha)
            .ToListAsync();
}
