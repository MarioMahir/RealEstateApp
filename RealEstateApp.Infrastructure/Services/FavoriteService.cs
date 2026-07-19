using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Interfaces.Repositories;
using RealEstateApp.Core.Interfaces.Services;

namespace RealEstateApp.Infrastructure.Services;

public class FavoriteService : IFavoriteService
{
    private readonly IGenericRepository<Favorite> _favoriteRepository;
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FavoriteService(
        IGenericRepository<Favorite> favoriteRepository,
        IGenericRepository<Property> propertyRepository,
        IUnitOfWork unitOfWork)
    {
        _favoriteRepository = favoriteRepository;
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<HashSet<int>> GetFavoritePropertyIdsAsync(string clienteId)
    {
        var ids = await _favoriteRepository.Query()
            .Where(f => f.ClienteId == clienteId)
            .Select(f => f.PropertyId)
            .ToListAsync();

        return ids.ToHashSet();
    }

    public async Task<bool?> ToggleFavoriteAsync(string clienteId, int propertyId)
    {
        var existente = await _favoriteRepository.GetByIdAsync(clienteId, propertyId);
        if (existente is not null)
        {
            _favoriteRepository.Delete(existente);
            await _unitOfWork.SaveChangesAsync();
            return false;
        }

        var propiedadExiste = await _propertyRepository.Query().AnyAsync(p => p.Id == propertyId);
        if (!propiedadExiste)
            return null;

        await _favoriteRepository.AddAsync(new Favorite { ClienteId = clienteId, PropertyId = propertyId });
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
