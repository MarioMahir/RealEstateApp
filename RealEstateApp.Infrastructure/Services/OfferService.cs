using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Enums;
using RealEstateApp.Core.Interfaces.Repositories;
using RealEstateApp.Core.Interfaces.Services;

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

    public async Task AcceptOfferAsync(int offerId)
    {
        var offer = await Repository.GetByIdAsync(offerId)
            ?? throw new KeyNotFoundException("La oferta solicitada no existe.");

        var pendientes = Repository.Query()
            .Where(o => o.PropertyId == offer.PropertyId && o.Estado == OfferStatus.Pendiente)
            .ToList();

        foreach (var pendiente in pendientes)
        {
            pendiente.Estado = pendiente.Id == offer.Id ? OfferStatus.Aceptada : OfferStatus.Rechazada;
            Repository.Update(pendiente);
        }

        var property = await _propertyRepository.GetByIdAsync(offer.PropertyId)
            ?? throw new KeyNotFoundException("La propiedad asociada no existe.");
        property.Estado = PropertyStatus.Vendida;
        _propertyRepository.Update(property);

        // Un solo SaveChanges: EF Core lo envuelve en una transaccion implicita,
        // por lo que la oferta aceptada, las rechazadas y el cambio de estado de
        // la propiedad se confirman (o fallan) todos juntos.
        await UnitOfWork.SaveChangesAsync();
    }

    public async Task RejectOfferAsync(int offerId)
    {
        var offer = await Repository.GetByIdAsync(offerId)
            ?? throw new KeyNotFoundException("La oferta solicitada no existe.");

        offer.Estado = OfferStatus.Rechazada;
        Repository.Update(offer);
        await UnitOfWork.SaveChangesAsync();
    }
}
