using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Enums;
using RealEstateApp.Core.Interfaces.Repositories;
using RealEstateApp.Core.Interfaces.Services;
using RealEstateApp.Core.Models;

namespace RealEstateApp.Infrastructure.Services;

public class PropertyService : GenericService<Property>, IPropertyService
{
    public PropertyService(IGenericRepository<Property> repository, IUnitOfWork unitOfWork)
        : base(repository, unitOfWork)
    {
    }

    public async Task<List<Property>> GetAllWithDetailsAsync() =>
        await WithDetails(Repository.Query()).ToListAsync();

    public async Task<Property?> GetByIdWithDetailsAsync(int id) =>
        await WithDetails(Repository.Query()).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Property?> GetByCodeWithDetailsAsync(string codigo) =>
        await WithDetails(Repository.Query()).FirstOrDefaultAsync(p => p.Codigo == codigo);

    public async Task<List<Property>> SearchAvailableAsync(PropertyFilterCriteria criteria)
    {
        var query = ApplyFilters(WithDetails(Repository.Query()).Where(p => p.Estado == PropertyStatus.Disponible), criteria);
        return await query.OrderByDescending(p => p.FechaCreacion).ToListAsync();
    }

    public async Task<Property?> GetAvailableByIdAsync(int id) =>
        await WithDetails(Repository.Query())
            .FirstOrDefaultAsync(p => p.Id == id && p.Estado == PropertyStatus.Disponible);

    public async Task<Property?> GetAvailableByCodeAsync(string codigo) =>
        await WithDetails(Repository.Query())
            .FirstOrDefaultAsync(p => p.Codigo == codigo && p.Estado == PropertyStatus.Disponible);

    public async Task<List<Property>> GetAvailableByAgentAsync(string agentId, PropertyFilterCriteria? criteria = null)
    {
        var query = WithDetails(Repository.Query())
            .Where(p => p.AgentId == agentId && p.Estado == PropertyStatus.Disponible);
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

    // Las consultas genericas (GetAllAsync/GetByIdAsync heredados) no cargan
    // relaciones; PropertyDto/los ViewModels necesitan PropertyType/SaleType/
    // Agent/Imagenes/Mejoras, asi que estas consultas especificas si las
    // incluyen explicitamente.
    private static IQueryable<Property> WithDetails(IQueryable<Property> query) =>
        query
            .Include(p => p.PropertyType)
            .Include(p => p.SaleType)
            .Include(p => p.Agent)
            .Include(p => p.Images)
            .Include(p => p.PropertyImprovements).ThenInclude(pi => pi.Improvement);
}
