using RealEstateApp.Core.Interfaces.Repositories;
using RealEstateApp.Core.Interfaces.Services;

namespace RealEstateApp.Infrastructure.Services;

public class GenericService<TEntity> : IGenericService<TEntity> where TEntity : class
{
    protected readonly IGenericRepository<TEntity> Repository;
    protected readonly IUnitOfWork UnitOfWork;

    public GenericService(IGenericRepository<TEntity> repository, IUnitOfWork unitOfWork)
    {
        Repository = repository;
        UnitOfWork = unitOfWork;
    }

    public Task<List<TEntity>> GetAllAsync() => Repository.GetAllAsync();

    public Task<TEntity?> GetByIdAsync(params object[] keys) => Repository.GetByIdAsync(keys);

    public async Task AddAsync(TEntity entity)
    {
        await Repository.AddAsync(entity);
        await UnitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(TEntity entity)
    {
        Repository.Update(entity);
        await UnitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(TEntity entity)
    {
        Repository.Delete(entity);
        await UnitOfWork.SaveChangesAsync();
    }
}
