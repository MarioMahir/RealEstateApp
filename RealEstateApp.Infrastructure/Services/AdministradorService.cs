using Microsoft.AspNetCore.Identity;
using RealEstateApp.Core.Constants;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Enums;
using RealEstateApp.Core.Interfaces.Repositories;
using RealEstateApp.Core.Interfaces.Services;
using RealEstateApp.Core.ViewModels.Administrador;

namespace RealEstateApp.Infrastructure.Services;

public class AdministradorService : IAdministradorService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IGenericRepository<Property> _propertyRepository;

    public AdministradorService(UserManager<ApplicationUser> userManager, IGenericRepository<Property> propertyRepository)
    {
        _userManager = userManager;
        _propertyRepository = propertyRepository;
    }

    public async Task<AdministradorHomeViewModel> GetIndicadoresAsync()
    {
        var agentes = await _userManager.GetUsersInRoleAsync(Roles.Agente);
        var clientes = await _userManager.GetUsersInRoleAsync(Roles.Cliente);
        var desarrolladores = await _userManager.GetUsersInRoleAsync(Roles.Desarrollador);

        return new AdministradorHomeViewModel
        {
            PropiedadesDisponibles = _propertyRepository.Query().Count(p => p.Estado == PropertyStatus.Disponible),
            PropiedadesVendidas = _propertyRepository.Query().Count(p => p.Estado == PropertyStatus.Vendida),
            AgentesActivos = agentes.Count(a => a.Activo),
            AgentesInactivos = agentes.Count(a => !a.Activo),
            ClientesActivos = clientes.Count(c => c.Activo),
            ClientesInactivos = clientes.Count(c => !c.Activo),
            DesarrolladoresActivos = desarrolladores.Count(d => d.Activo),
            DesarrolladoresInactivos = desarrolladores.Count(d => !d.Activo)
        };
    }
}
