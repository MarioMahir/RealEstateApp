using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Interfaces.Repositories;
using RealEstateApp.Core.Interfaces.Services;

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

    // Las consultas genericas (GetAllAsync/GetByIdAsync heredados) no cargan
    // relaciones; PropertyDto necesita PropertyType/SaleType/Agent/Mejoras, asi
    // que estas consultas especificas si las incluyen explicitamente.
    private static IQueryable<Property> WithDetails(IQueryable<Property> query) =>
        query
            .Include(p => p.PropertyType)
            .Include(p => p.SaleType)
            .Include(p => p.Agent)
            .Include(p => p.PropertyImprovements).ThenInclude(pi => pi.Improvement);
}
