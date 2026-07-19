using RealEstateApp.Core.Entities;

namespace RealEstateApp.Core.Interfaces.Services;

public interface IPropertyService : IGenericService<Property>
{
    // Devuelven la entidad con PropertyType/SaleType/Agent/Mejoras ya cargados
    // (Include), lista para mapear a PropertyDto en el controlador. El
    // IGenericService.GetByIdAsync heredado NO carga esas relaciones.
    Task<List<Property>> GetAllWithDetailsAsync();
    Task<Property?> GetByIdWithDetailsAsync(int id);
    Task<Property?> GetByCodeWithDetailsAsync(string codigo);
}
