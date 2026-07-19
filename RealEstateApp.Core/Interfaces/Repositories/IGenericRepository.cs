namespace RealEstateApp.Core.Interfaces.Repositories;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<List<TEntity>> GetAllAsync();

    // params soporta claves primarias compuestas (ej. Favorite: ClienteId + PropertyId).
    Task<TEntity?> GetByIdAsync(params object[] keys);

    // Para filtros/Include especificos que el generico no cubre.
    IQueryable<TEntity> Query();

    Task AddAsync(TEntity entity);

    // No persiste por si sola: solo marca el change tracker. Quien orquesta
    // cuando se guarda es el servicio, via IUnitOfWork.
    void Update(TEntity entity);
    void Delete(TEntity entity);
}
