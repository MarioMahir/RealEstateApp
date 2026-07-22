using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Models;

namespace RealEstateApp.Core.Interfaces.Services;

public interface IAccountService
{
    Task<CredentialValidationResult> ValidateCredentialsAsync(string usernameOrEmail, string password);
    Task<IList<string>> GetRolesAsync(ApplicationUser user);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> CedulaExistsAsync(string cedula);
    Task<RegisterResult> RegisterAsync(ApplicationUser user, string password, string role);

    Task<ApplicationUser?> FindByIdAsync(string userId);
    Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
    Task<bool> ConfirmEmailAndActivateAsync(ApplicationUser user, string token);
}
