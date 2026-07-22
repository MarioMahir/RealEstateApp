using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Constants;
using RealEstateApp.Core.DTOs.Account;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Interfaces.Services;
using RealEstateApp.Core.Models;
using RealEstateApp.WebAPI.Services;

namespace RealEstateApp.WebAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IJwtTokenService _jwtTokenService;

    public AccountController(IAccountService accountService, IJwtTokenService jwtTokenService)
    {
        _accountService = accountService;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginRequestDto dto)
    {
        var result = await _accountService.ValidateCredentialsAsync(dto.UsuarioOCorreo, dto.Contrasena);

        if (result.Status == CredentialValidationStatus.InvalidCredentials)
            return Unauthorized(new { message = "Los datos de acceso son inválidos." });

        if (result.Status == CredentialValidationStatus.Inactive)
            return Unauthorized(new { message = "El usuario se encuentra inactivo y no puede autenticarse." });

        var roles = await _accountService.GetRolesAsync(result.User!);

        if (!roles.Contains(Roles.Administrador) && !roles.Contains(Roles.Desarrollador))
            return StatusCode(StatusCodes.Status403Forbidden,
                new { message = "Acceso denegado. No tiene permisos para realizar esta acción." });

        var (token, expiration) = _jwtTokenService.GenerateToken(result.User!, roles);

        return Ok(new LoginResponseDto
        {
            Token = token,
            Usuario = result.User!.UserName ?? result.User.Email ?? string.Empty,
            Roles = roles,
            Expiracion = expiration
        });
    }

    [HttpPost("RegisterDeveloper")]
    [Authorize(Roles = Roles.Administrador)]
    public Task<IActionResult> RegisterDeveloper(RegisterDeveloperDto dto) =>
        RegisterAsync(dto, Roles.Desarrollador);

    [HttpPost("RegisterAdmin")]
    [Authorize(Roles = Roles.Administrador)]
    public Task<IActionResult> RegisterAdmin(RegisterAdminDto dto) =>
        RegisterAsync(dto, Roles.Administrador);

    private async Task<IActionResult> RegisterAsync(RegisterUserDto dto, string role)
    {
        if (await _accountService.CedulaExistsAsync(dto.Cedula))
            return BadRequest(new { message = "Ya existe un usuario registrado con esta cédula." });

        if (await _accountService.EmailExistsAsync(dto.CorreoElectronico))
            return BadRequest(new { message = "Ya existe un usuario registrado con este correo electrónico." });

        if (await _accountService.UsernameExistsAsync(dto.NombreUsuario))
            return BadRequest(new { message = "Ya existe un usuario registrado con este nombre de usuario." });

        var user = new ApplicationUser
        {
            UserName = dto.NombreUsuario,
            Email = dto.CorreoElectronico,
            EmailConfirmed = true,
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Cedula = dto.Cedula,
            Activo = true
        };

        var result = await _accountService.RegisterAsync(user, dto.Contrasena, role);
        if (!result.Succeeded)
            return BadRequest(new { message = "Los datos enviados no son válidos.", errores = result.Errors });

        return StatusCode(StatusCodes.Status201Created, new { id = user.Id, usuario = user.UserName, rol = role });
    }
}
