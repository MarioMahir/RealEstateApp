using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Constants;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Interfaces.Services;
using RealEstateApp.Core.Models;
using RealEstateApp.Core.ViewModels.Account;

namespace RealEstateApp.WebApp.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IEmailService _emailService;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        IAccountService accountService,
        IFileStorageService fileStorageService,
        IEmailService emailService,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger)
    {
        _accountService = accountService;
        _fileStorageService = fileStorageService;
        _emailService = emailService;
        _signInManager = signInManager;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity!.IsAuthenticated)
            return RedirectToAction(nameof(Login));

        return View(new RegisterViewModel());
    }

    // Registro publico: solo Cliente o Agente (Administrador/Desarrollador se
    // crean exclusivamente desde el panel de administracion, Etapa 5).
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel modelo, IFormFile? foto)
    {
        if (modelo.TipoUsuario != Roles.Cliente && modelo.TipoUsuario != Roles.Agente)
            ModelState.AddModelError(nameof(modelo.TipoUsuario), "Debe seleccionar un tipo de usuario válido.");

        if (await _accountService.EmailExistsAsync(modelo.CorreoElectronico))
            ModelState.AddModelError(nameof(modelo.CorreoElectronico), "Ya existe un usuario registrado con este correo electrónico.");

        if (await _accountService.UsernameExistsAsync(modelo.NombreUsuario))
            ModelState.AddModelError(nameof(modelo.NombreUsuario), "Ya existe un usuario registrado con este nombre de usuario.");

        if (foto is not { Length: > 0 })
            ModelState.AddModelError(string.Empty, "La foto de usuario es requerida.");

        if (!ModelState.IsValid)
            return View(modelo);

        string? fotoUrl = null;
        if (foto is { Length: > 0 })
        {
            try
            {
                fotoUrl = await _fileStorageService.SaveImageAsync(foto.OpenReadStream(), foto.FileName, "usuarios");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(modelo);
            }
        }

        var usuario = new ApplicationUser
        {
            UserName = modelo.NombreUsuario,
            Email = modelo.CorreoElectronico,
            PhoneNumber = modelo.Telefono,
            Nombre = modelo.Nombre,
            Apellido = modelo.Apellido,
            FotoUrl = fotoUrl,
            Activo = false
        };

        var resultado = await _accountService.RegisterAsync(usuario, modelo.Contrasena, modelo.TipoUsuario);
        if (!resultado.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "No fue posible completar el registro. Intente nuevamente más tarde.");
            return View(modelo);
        }

        if (modelo.TipoUsuario == Roles.Cliente)
        {
            var correoEnviado = await EnviarCorreoActivacionAsync(usuario);
            TempData["Mensaje"] = correoEnviado
                ? "Su cuenta ha sido creada correctamente. Revise su correo electrónico para activar su usuario."
                : "Su cuenta fue creada, pero no se pudo enviar el correo de activación. Contacte al administrador para activarla.";
        }
        else
        {
            TempData["Mensaje"] = "Su cuenta de agente ha sido creada correctamente. Un administrador debe activar su usuario antes de que pueda iniciar sesión.";
        }

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl)
    {
        if (User.Identity!.IsAuthenticated)
            return RedirectToAction(nameof(Index), "Home");

        // El esquema de cookies redirige aqui con ?ReturnUrl= cuando un
        // usuario no autenticado intenta acceder directamente a una pantalla
        // privada -- distinto de llegar por el enlace normal "Iniciar sesion".
        if (!string.IsNullOrEmpty(returnUrl))
            ViewData["Mensaje"] = "Debe iniciar sesión para acceder a esta funcionalidad.";

        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel modelo)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        var resultado = await _accountService.ValidateCredentialsAsync(modelo.UsuarioOCorreo, modelo.Contrasena);

        if (resultado.Status == CredentialValidationStatus.InvalidCredentials)
        {
            ModelState.AddModelError(string.Empty, "Los datos de acceso son inválidos.");
            return View(modelo);
        }

        if (resultado.Status == CredentialValidationStatus.Inactive)
        {
            ModelState.AddModelError(string.Empty, "El usuario se encuentra inactivo y no puede iniciar sesión.");
            return View(modelo);
        }

        var roles = await _accountService.GetRolesAsync(resultado.User!);

        // Desarrollador es un rol exclusivo de la WebAPI -- credenciales
        // correctas, pero sin acceso aqui (mensaje propio, no un 401/403 crudo
        // como en la WebAPI).
        if (!roles.Contains(Roles.Administrador) && !roles.Contains(Roles.Cliente) && !roles.Contains(Roles.Agente))
        {
            ModelState.AddModelError(string.Empty, "El usuario no tiene un rol válido asignado. Póngase en contacto con un administrador.");
            return View(modelo);
        }

        await _signInManager.SignInAsync(resultado.User!, isPersistent: false);

        return RedirectToRoleHome(roles);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Index), "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    // Consumido desde el enlace del correo de activacion (Cliente). Reutiliza
    // el token de confirmacion nativo de Identity; ver AccountService.
    [HttpGet]
    public async Task<IActionResult> ActivateAccount(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            return RedirectToAction(nameof(Login));

        var usuario = await _accountService.FindByIdAsync(userId);
        if (usuario is null)
        {
            TempData["Error"] = "El enlace de activación no es válido.";
            return RedirectToAction(nameof(Login));
        }

        var activado = await _accountService.ConfirmEmailAndActivateAsync(usuario, token);
        TempData[activado ? "Mensaje" : "Error"] = activado
            ? "Su cuenta fue activada correctamente. Ya puede iniciar sesión."
            : "El enlace de activación no es válido o ya expiró.";

        return RedirectToAction(nameof(Login));
    }

    private async Task<bool> EnviarCorreoActivacionAsync(ApplicationUser usuario)
    {
        var token = await _accountService.GenerateEmailConfirmationTokenAsync(usuario);
        var enlace = Url.Action(nameof(ActivateAccount), "Account", new { userId = usuario.Id, token }, Request.Scheme);

        try
        {
            await _emailService.SendEmailAsync(
                usuario.Email!,
                "Activación de cuenta en RealEstateApp",
                $"<p>Hola {usuario.Nombre},</p>" +
                "<p>Su cuenta ha sido registrada correctamente en RealEstateApp.</p>" +
                $"<p>Para activar su usuario y poder iniciar sesión, utilice el siguiente enlace de activación: <a href=\"{enlace}\">{enlace}</a></p>" +
                "<p>Si usted no realizó este registro, puede ignorar este mensaje.</p>");
            return true;
        }
        catch (Exception ex)
        {
            // Un fallo de SMTP (credenciales/host aun no configurados, corte de
            // red, etc.) no debe tumbar el registro, que ya se persistio: la
            // cuenta simplemente queda a la espera de un correo que no llego.
            // El enlace se deja en el log para poder activar manualmente en dev.
            _logger.LogWarning(ex, "No se pudo enviar el correo de activación a {Email}. Enlace: {Enlace}", usuario.Email, enlace);
            return false;
        }
    }

    private IActionResult RedirectToRoleHome(IList<string> roles)
    {
        if (roles.Contains(Roles.Administrador))
            return RedirectToAction(nameof(AdministradorController.Index), "Administrador");
        if (roles.Contains(Roles.Agente))
            return RedirectToAction(nameof(AgenteController.Index), "Agente");

        return RedirectToAction(nameof(Index), "Home");
    }
}
