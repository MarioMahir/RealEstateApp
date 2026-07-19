using RealEstateApp.Core.Interfaces.Services;
using RealEstateApp.Infrastructure.Persistence;

namespace RealEstateApp.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
}
