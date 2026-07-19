using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Interfaces.Services;
using RealEstateApp.Core.Models;

namespace RealEstateApp.Infrastructure.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<CredentialValidationResult> ValidateCredentialsAsync(string usernameOrEmail, string password)
    {
        var user = await _userManager.FindByNameAsync(usernameOrEmail)
            ?? await _userManager.FindByEmailAsync(usernameOrEmail);

        if (user is null || !await _userManager.CheckPasswordAsync(user, password))
            return new CredentialValidationResult { Status = CredentialValidationStatus.InvalidCredentials };

        if (!user.Activo)
            return new CredentialValidationResult { Status = CredentialValidationStatus.Inactive, User = user };

        return new CredentialValidationResult { Status = CredentialValidationStatus.Success, User = user };
    }

    public Task<IList<string>> GetRolesAsync(ApplicationUser user) => _userManager.GetRolesAsync(user);

    public async Task<bool> EmailExistsAsync(string email) => await _userManager.FindByEmailAsync(email) is not null;

    public async Task<bool> UsernameExistsAsync(string username) => await _userManager.FindByNameAsync(username) is not null;

    public Task<bool> CedulaExistsAsync(string cedula) => _userManager.Users.AnyAsync(u => u.Cedula == cedula);

    public async Task<RegisterResult> RegisterAsync(ApplicationUser user, string password, string role)
    {
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return new RegisterResult { Succeeded = false, Errors = result.Errors.Select(e => e.Description).ToList() };

        await _userManager.AddToRoleAsync(user, role);
        return new RegisterResult { Succeeded = true };
    }

    public Task<ApplicationUser?> FindByIdAsync(string userId) => _userManager.FindByIdAsync(userId)!;

    public Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user) =>
        _userManager.GenerateEmailConfirmationTokenAsync(user);

    public async Task<bool> ConfirmEmailAndActivateAsync(ApplicationUser user, string token)
    {
        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
            return false;

        user.Activo = true;
        await _userManager.UpdateAsync(user);
        return true;
    }
}
