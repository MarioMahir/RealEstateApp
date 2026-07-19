namespace RealEstateApp.Core.Interfaces.Services;

// Unico punto donde se persiste realmente. Permite que un servicio combine
// varias operaciones de repositorio (ej. actualizar N ofertas + 1 propiedad)
// en una sola llamada a SaveChangesAsync, que EF Core envuelve en una
// transaccion implicita.
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}
