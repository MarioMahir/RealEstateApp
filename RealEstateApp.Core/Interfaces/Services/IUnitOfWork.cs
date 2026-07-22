namespace RealEstateApp.Core.Interfaces.Services;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}
