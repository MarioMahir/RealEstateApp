using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Enums;
using RealEstateApp.Core.Interfaces.Repositories;
using RealEstateApp.Core.Interfaces.Services;
using RealEstateApp.Core.Models;

namespace RealEstateApp.Infrastructure.Services;

public class PropertyService : GenericService<Property>, IPropertyService
{
    private readonly IGenericRepository<PropertyImage> _imageRepository;
    private readonly IGenericRepository<PropertyImprovement> _improvementLinkRepository;

    public PropertyService(
        IGenericRepository<Property> repository,
        IGenericRepository<PropertyImage> imageRepository,
        IGenericRepository<PropertyImprovement> improvementLinkRepository,
        IUnitOfWork unitOfWork)
        : base(repository, unitOfWork)
    {
        _imageRepository = imageRepository;
        _improvementLinkRepository = improvementLinkRepository;
    }

    public async Task<List<Property>> GetAllWithDetailsAsync() =>
        await WithDetails(Repository.Query()).ToListAsync();

    public async Task<Property?> GetByIdWithDetailsAsync(int id) =>
        await WithDetails(Repository.Query()).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Property?> GetByCodeWithDetailsAsync(string codigo) =>
        await WithDetails(Repository.Query()).FirstOrDefaultAsync(p => p.Codigo == codigo);

    public async Task<List<Property>> SearchAvailableAsync(PropertyFilterCriteria criteria)
    {
        var query = ApplyFilters(
            WithDetails(Repository.Query()).Where(p => p.Estado == PropertyStatus.Disponible && p.Agent.Activo),
            criteria);
        return await query.OrderByDescending(p => p.FechaCreacion).ToListAsync();
    }

    public async Task<Property?> GetAvailableByIdAsync(int id) =>
        await WithDetails(Repository.Query())
            .FirstOrDefaultAsync(p => p.Id == id && p.Estado == PropertyStatus.Disponible && p.Agent.Activo);

    public async Task<Property?> GetAvailableByCodeAsync(string codigo) =>
        await WithDetails(Repository.Query())
            .FirstOrDefaultAsync(p => p.Codigo == codigo && p.Estado == PropertyStatus.Disponible && p.Agent.Activo);

    public async Task<List<Property>> GetAvailableByAgentAsync(string agentId, PropertyFilterCriteria? criteria = null)
    {
        var query = WithDetails(Repository.Query())
            .Where(p => p.AgentId == agentId && p.Estado == PropertyStatus.Disponible && p.Agent.Activo);
        if (criteria is not null)
            query = ApplyFilters(query, criteria);

        return await query.OrderByDescending(p => p.FechaCreacion).ToListAsync();
    }

    private static IQueryable<Property> ApplyFilters(IQueryable<Property> query, PropertyFilterCriteria criteria)
    {
        if (criteria.PropertyTypeId.HasValue)
            query = query.Where(p => p.PropertyTypeId == criteria.PropertyTypeId);
        if (criteria.PrecioMinimo.HasValue)
            query = query.Where(p => p.Precio >= criteria.PrecioMinimo);
        if (criteria.PrecioMaximo.HasValue)
            query = query.Where(p => p.Precio <= criteria.PrecioMaximo);
        if (criteria.CantidadHabitaciones.HasValue)
            query = query.Where(p => p.CantidadHabitaciones == criteria.CantidadHabitaciones);
        if (criteria.CantidadBanos.HasValue)
            query = query.Where(p => p.CantidadBanos == criteria.CantidadBanos);

        return query;
    }

    public async Task<List<Property>> GetAllByAgentAsync(string agentId) =>
        await WithDetails(Repository.Query())
            .Where(p => p.AgentId == agentId)
            .OrderByDescending(p => p.FechaCreacion)
            .ToListAsync();

    public async Task<List<Property>> GetFavoritesByClienteAsync(string clienteId) =>
        await WithDetails(Repository.Query())
            .Where(p => p.Estado == PropertyStatus.Disponible && p.Agent.Activo && p.Favorites.Any(f => f.ClienteId == clienteId))
            .OrderByDescending(p => p.FechaCreacion)
            .ToListAsync();

    public async Task<Property?> GetByIdForAgentAsync(int id, string agentId) =>
        await WithDetails(Repository.Query()).FirstOrDefaultAsync(p => p.Id == id && p.AgentId == agentId);

    public async Task<string> GenerateUniqueCodeAsync()
    {
        string codigo;
        do
        {
            codigo = Random.Shared.Next(0, 1_000_000).ToString("D6");
        }
        while (await Repository.Query().AnyAsync(p => p.Codigo == codigo));

        return codigo;
    }

    public async Task<Property> CreatePropertyAsync(Property property, List<int> improvementIds, List<string> imageUrls)
    {
        property.Codigo = await GenerateUniqueCodeAsync();
        property.FechaCreacion = DateTime.Now;
        property.Estado = PropertyStatus.Disponible;

        foreach (var url in imageUrls)
            property.Images.Add(new PropertyImage { Url = url });

        foreach (var improvementId in improvementIds)
            property.PropertyImprovements.Add(new PropertyImprovement { ImprovementId = improvementId });

        await Repository.AddAsync(property);
        await UnitOfWork.SaveChangesAsync();
        return property;
    }

    public async Task<bool> UpdatePropertyAsync(
        Property cambios, List<int> improvementIds, List<int> imageIdsToRemove, List<string> newImageUrls)
    {
        var existente = await WithDetails(Repository.Query())
            .FirstOrDefaultAsync(p => p.Id == cambios.Id && p.AgentId == cambios.AgentId);
        if (existente is null || existente.Estado != PropertyStatus.Disponible)
            return false;

        existente.PropertyTypeId = cambios.PropertyTypeId;
        existente.SaleTypeId = cambios.SaleTypeId;
        existente.Precio = cambios.Precio;
        existente.TamanoTerreno = cambios.TamanoTerreno;
        existente.CantidadHabitaciones = cambios.CantidadHabitaciones;
        existente.CantidadBanos = cambios.CantidadBanos;
        existente.Descripcion = cambios.Descripcion;

        foreach (var imagen in existente.Images.Where(i => imageIdsToRemove.Contains(i.Id)).ToList())
            _imageRepository.Delete(imagen);

        foreach (var url in newImageUrls)
            existente.Images.Add(new PropertyImage { Url = url });

        var idsActuales = existente.PropertyImprovements.Select(pi => pi.ImprovementId).ToHashSet();
        var idsNuevos = improvementIds.ToHashSet();

        foreach (var vinculo in existente.PropertyImprovements.Where(pi => !idsNuevos.Contains(pi.ImprovementId)).ToList())
            _improvementLinkRepository.Delete(vinculo);

        foreach (var improvementId in idsNuevos.Except(idsActuales))
            existente.PropertyImprovements.Add(new PropertyImprovement { PropertyId = existente.Id, ImprovementId = improvementId });

        Repository.Update(existente);
        await UnitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePropertyAsync(int propertyId, string agentId)
    {
        var propiedad = await Repository.Query().FirstOrDefaultAsync(p => p.Id == propertyId && p.AgentId == agentId);
        if (propiedad is null || propiedad.Estado != PropertyStatus.Disponible)
            return false;

        Repository.Delete(propiedad);
        await UnitOfWork.SaveChangesAsync();
        return true;
    }

    private static IQueryable<Property> WithDetails(IQueryable<Property> query) =>
        query
            .Include(p => p.PropertyType)
            .Include(p => p.SaleType)
            .Include(p => p.Agent)
            .Include(p => p.Images)
            .Include(p => p.PropertyImprovements).ThenInclude(pi => pi.Improvement);
}
