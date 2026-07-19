using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Interfaces.Repositories;
using RealEstateApp.Infrastructure.Persistence;

namespace RealEstateApp.Infrastructure.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<List<TEntity>> GetAllAsync() => await _dbSet.ToListAsync();

    public async Task<TEntity?> GetByIdAsync(params object[] keys) => await _dbSet.FindAsync(keys);

    public IQueryable<TEntity> Query() => _dbSet.AsQueryable();

    public async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);

    public void Update(TEntity entity) => _dbSet.Update(entity);

    public void Delete(TEntity entity) => _dbSet.Remove(entity);
}
