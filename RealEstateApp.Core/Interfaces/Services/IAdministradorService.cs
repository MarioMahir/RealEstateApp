using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Models;
using RealEstateApp.Core.ViewModels.Administrador;

namespace RealEstateApp.Core.Interfaces.Services;

public interface IAdministradorService
{
    Task<AdministradorHomeViewModel> GetIndicadoresAsync();

    // "Mantenimiento de administradores" / "Mantenimiento de desarrolladores":
    // todos los usuarios de ese rol (activos e inactivos).
    Task<List<ApplicationUser>> GetUsersInRoleAsync(string role);

    // null = no existe o no tiene el rol indicado -- evita que, por ejemplo,
    // Mantenimiento de administradores pueda editar/activar un usuario que en
    // realidad es Cliente o Agente (mismo patron que AgentService.
    // GetAgentUserOrNullAsync).
    Task<ApplicationUser?> GetStaffUserAsync(string userId, string role);

    // enforceSelfProtection = true solo para Administrador (Desarrollador no
    // tiene estas reglas): no editar el propio usuario.
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

    // enforceSelfProtection = true solo para Administrador: no inactivar el
    // propio usuario, y nunca dejar el sistema con 0 administradores activos.
    Task<StaffActionStatus> ToggleStaffStatusAsync(
        string targetUserId, string currentUserId, string role, bool enforceSelfProtection);
}
