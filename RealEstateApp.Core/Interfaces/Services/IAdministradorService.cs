using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Models;
using RealEstateApp.Core.ViewModels.Administrador;

namespace RealEstateApp.Core.Interfaces.Services;

public interface IAdministradorService
{
    Task<AdministradorHomeViewModel> GetIndicadoresAsync();

    Task<List<ApplicationUser>> GetUsersInRoleAsync(string role);

    Task<ApplicationUser?> GetStaffUserAsync(string userId, string role);

    Task<StaffUpdateResult> UpdateStaffUserAsync(
        string targetUserId,
        string currentUserId,
        string role,
        bool enforceSelfProtection,
        string nombre,
        string apellido,
        string cedula,
        string correo,
        string nombreUsuario,
        string? nuevaContrasena);

    Task<StaffActionStatus> ToggleStaffStatusAsync(
        string targetUserId, string currentUserId, string role, bool enforceSelfProtection);
}
