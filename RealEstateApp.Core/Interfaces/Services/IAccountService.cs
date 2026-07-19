using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Models;

namespace RealEstateApp.Core.Interfaces.Services;

// Operaciones de Identity compartidas por WebApp y WebAPI. No sabe nada de JWT
// ni de cookies -- eso es responsabilidad exclusiva de cada host (ver
// JwtTokenService en RealEstateApp.WebAPI).
public interface IAccountService
{
    Task<CredentialValidationResult> ValidateCredentialsAsync(string usernameOrEmail, string password);
    Task<IList<string>> GetRolesAsync(ApplicationUser user);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> CedulaExistsAsync(string cedula);
    Task<RegisterResult> RegisterAsync(ApplicationUser user, string password, string role);

    // Activacion de cuenta de Cliente (correo). Reutiliza el generador de
    // tokens de ASP.NET Identity en vez de inventar uno propio; el flag de
    // negocio "Activo" (no "EmailConfirmed") sigue siendo lo unico que el
    // login revisa.
    Task<ApplicationUser?> FindByIdAsync(string userId);
    Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
    Task<bool> ConfirmEmailAndActivateAsync(ApplicationUser user, string token);
}
