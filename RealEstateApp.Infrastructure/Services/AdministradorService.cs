using Microsoft.AspNetCore.Identity;
using RealEstateApp.Core.Constants;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Enums;
using RealEstateApp.Core.Interfaces.Repositories;
using RealEstateApp.Core.Interfaces.Services;
using RealEstateApp.Core.Models;
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

    public async Task<List<ApplicationUser>> GetUsersInRoleAsync(string role) =>
        (await _userManager.GetUsersInRoleAsync(role)).ToList();

    public async Task<StaffUpdateResult> UpdateStaffUserAsync(
        string targetUserId,
        string currentUserId,
        bool enforceSelfProtection,
        string nombre,
        string apellido,
        string cedula,
        string correo,
        string nombreUsuario,
        string? nuevaContrasena)
    {
        if (enforceSelfProtection && targetUserId == currentUserId)
            return new StaffUpdateResult { Status = StaffActionStatus.CannotModifySelf };

        var usuario = await _userManager.FindByIdAsync(targetUserId);
        if (usuario is null)
            return new StaffUpdateResult { Status = StaffActionStatus.NotFound };

        // La contrasena se intenta primero: si falla (no cumple las reglas de
        // Identity), no se toca ningun otro campo -- evita un estado
        // parcialmente actualizado.
        if (!string.IsNullOrWhiteSpace(nuevaContrasena))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
            var resultadoPassword = await _userManager.ResetPasswordAsync(usuario, token, nuevaContrasena);
            if (!resultadoPassword.Succeeded)
                return new StaffUpdateResult
                {
                    Status = StaffActionStatus.PasswordUpdateFailed,
                    Errors = resultadoPassword.Errors.Select(e => e.Description).ToList()
                };
        }

        usuario.Nombre = nombre;
        usuario.Apellido = apellido;
        usuario.Cedula = cedula;
        await _userManager.UpdateAsync(usuario);

        // Email/UserName tienen su propio campo "Normalized*" que UserManager
        // mantiene sincronizado solo si se usan estos metodos dedicados (asignar
        // ApplicationUser.Email/UserName directo dejaria la busqueda por
        // FindByEmailAsync/FindByNameAsync rota).
        await _userManager.SetEmailAsync(usuario, correo);
        await _userManager.SetUserNameAsync(usuario, nombreUsuario);

        return new StaffUpdateResult { Status = StaffActionStatus.Success };
    }

    public async Task<StaffActionStatus> ToggleStaffStatusAsync(
        string targetUserId, string currentUserId, string role, bool enforceSelfProtection)
    {
        if (enforceSelfProtection && targetUserId == currentUserId)
            return StaffActionStatus.CannotModifySelf;

        var usuario = await _userManager.FindByIdAsync(targetUserId);
        if (usuario is null)
            return StaffActionStatus.NotFound;

        var activar = !usuario.Activo;

        if (enforceSelfProtection && !activar)
        {
            var usuarios = await _userManager.GetUsersInRoleAsync(role);
            var activosRestantes = usuarios.Count(u => u.Activo && u.Id != targetUserId);
            if (activosRestantes == 0)
                return StaffActionStatus.LastActiveAdmin;
        }

        usuario.Activo = activar;
        await _userManager.UpdateAsync(usuario);
        return StaffActionStatus.Success;
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
