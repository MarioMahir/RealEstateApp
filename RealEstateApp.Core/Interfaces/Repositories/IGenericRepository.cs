namespace RealEstateApp.Core.Interfaces.Repositories;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<List<TEntity>> GetAllAsync();

    Task<TEntity?> GetByIdAsync(params object[] keys);

    IQueryable<TEntity> Query();

    Task AddAsync(TEntity entity);

    void Update(TEntity entity);
    void Delete(TEntity entity);
}
