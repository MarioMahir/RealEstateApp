using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Constants;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Interfaces.Services;
using RealEstateApp.Core.Models;
using RealEstateApp.Core.ViewModels.Administrador;

namespace RealEstateApp.WebApp.Controllers;

[Authorize(Roles = Roles.Administrador)]
public abstract class StaffMaintenanceControllerBase : Controller
{
    protected readonly IAdministradorService AdministradorService;
    protected readonly IAccountService AccountService;
    protected readonly IMapper Mapper;

    protected StaffMaintenanceControllerBase(
        IAdministradorService administradorService, IAccountService accountService, IMapper mapper)
    {
        AdministradorService = administradorService;
        AccountService = accountService;
        Mapper = mapper;
    }

    protected abstract string Rol { get; }
    protected abstract bool AplicaAutoproteccion { get; }

    protected abstract string Titulo { get; }
    protected abstract string EtiquetaCrear { get; }

    protected abstract string EntidadSingular { get; }
    protected abstract string EmptyListMessage { get; }
    protected abstract string NotFoundMessage { get; }
    protected abstract string CreateSuccessMessage { get; }
    protected abstract string UpdateSuccessMessage { get; }
    protected abstract string ActivateSuccessMessage { get; }
    protected abstract string DeactivateSuccessMessage { get; }

    protected virtual string CannotModifySelfMessage => string.Empty;
    protected virtual string CannotDeactivateSelfMessage => string.Empty;
    protected virtual string LastActiveMessage => string.Empty;

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public async Task<IActionResult> Index()
    {
        CargarViewData();
        var usuarios = await AdministradorService.GetUsersInRoleAsync(Rol);
        var modelo = Mapper.Map<List<UsuarioListItemViewModel>>(
            usuarios.OrderBy(u => u.Nombre).ThenBy(u => u.Apellido));

        return View("~/Views/Shared/Staff/Index.cshtml", modelo);
    }

    [HttpGet]
    public IActionResult Crear()
    {
        CargarViewData();
        return View("~/Views/Shared/Staff/Crear.cshtml", new CrearUsuarioViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CrearUsuarioViewModel modelo)
    {
        CargarViewData();

        if (!ModelState.IsValid)
            return View("~/Views/Shared/Staff/Crear.cshtml", modelo);

        if (await AccountService.CedulaExistsAsync(modelo.Cedula))
            ModelState.AddModelError(nameof(modelo.Cedula), "Ya existe un usuario registrado con esta cédula.");
        if (await AccountService.EmailExistsAsync(modelo.CorreoElectronico))
            ModelState.AddModelError(nameof(modelo.CorreoElectronico), "Ya existe un usuario registrado con este correo electrónico.");
        if (await AccountService.UsernameExistsAsync(modelo.NombreUsuario))
            ModelState.AddModelError(nameof(modelo.NombreUsuario), "Ya existe un usuario registrado con este nombre de usuario.");

        if (!ModelState.IsValid)
            return View("~/Views/Shared/Staff/Crear.cshtml", modelo);

        var usuario = new ApplicationUser
        {
            UserName = modelo.NombreUsuario,
            Email = modelo.CorreoElectronico,
            EmailConfirmed = true,
            Nombre = modelo.Nombre,
            Apellido = modelo.Apellido,
            Cedula = modelo.Cedula,
            Activo = true
        };

        var resultado = await AccountService.RegisterAsync(usuario, modelo.Contrasena, Rol);
        if (!resultado.Succeeded)
        {
            foreach (var error in resultado.Errors)
                ModelState.AddModelError(string.Empty, error);
            return View("~/Views/Shared/Staff/Crear.cshtml", modelo);
        }

        TempData["Mensaje"] = CreateSuccessMessage;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Editar(string id)
    {
        CargarViewData();

        if (AplicaAutoproteccion && id == CurrentUserId)
        {
            TempData["Error"] = CannotModifySelfMessage;
            return RedirectToAction(nameof(Index));
        }

        var usuario = await AdministradorService.GetStaffUserAsync(id, Rol);
        if (usuario is null)
        {
            TempData["Error"] = NotFoundMessage;
            return RedirectToAction(nameof(Index));
        }

        var modelo = new EditarUsuarioViewModel
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Cedula = usuario.Cedula ?? string.Empty,
            CorreoElectronico = usuario.Email ?? string.Empty,
            NombreUsuario = usuario.UserName ?? string.Empty
        };
        return View("~/Views/Shared/Staff/Editar.cshtml", modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(string id, EditarUsuarioViewModel modelo)
    {
        CargarViewData();

        if (AplicaAutoproteccion && id == CurrentUserId)
        {
            TempData["Error"] = CannotModifySelfMessage;
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            modelo.Id = id;
            return View("~/Views/Shared/Staff/Editar.cshtml", modelo);
        }

        var usuarioActual = await AdministradorService.GetStaffUserAsync(id, Rol);
        if (usuarioActual is null)
        {
            TempData["Error"] = NotFoundMessage;
            return RedirectToAction(nameof(Index));
        }

        if (!string.Equals(usuarioActual.Cedula, modelo.Cedula, StringComparison.Ordinal)
            && await AccountService.CedulaExistsAsync(modelo.Cedula))
            ModelState.AddModelError(nameof(modelo.Cedula), "Ya existe un usuario registrado con esta cédula.");

        if (!string.Equals(usuarioActual.Email, modelo.CorreoElectronico, StringComparison.OrdinalIgnoreCase)
            && await AccountService.EmailExistsAsync(modelo.CorreoElectronico))
            ModelState.AddModelError(nameof(modelo.CorreoElectronico), "Ya existe un usuario registrado con este correo electrónico.");

        if (!string.Equals(usuarioActual.UserName, modelo.NombreUsuario, StringComparison.OrdinalIgnoreCase)
            && await AccountService.UsernameExistsAsync(modelo.NombreUsuario))
            ModelState.AddModelError(nameof(modelo.NombreUsuario), "Ya existe un usuario registrado con este nombre de usuario.");

        if (!ModelState.IsValid)
        {
            modelo.Id = id;
            return View("~/Views/Shared/Staff/Editar.cshtml", modelo);
        }

        var resultado = await AdministradorService.UpdateStaffUserAsync(
            id, CurrentUserId, Rol, AplicaAutoproteccion,
            modelo.Nombre, modelo.Apellido, modelo.Cedula, modelo.CorreoElectronico, modelo.NombreUsuario, modelo.NuevaContrasena);

        if (resultado.Status == StaffActionStatus.PasswordUpdateFailed)
        {
            foreach (var error in resultado.Errors)
                ModelState.AddModelError(string.Empty, error);
            modelo.Id = id;
            return View("~/Views/Shared/Staff/Editar.cshtml", modelo);
        }

        if (resultado.Status != StaffActionStatus.Success)
        {
            TempData["Error"] = resultado.Status switch
            {
                StaffActionStatus.CannotModifySelf => CannotModifySelfMessage,
                StaffActionStatus.NotFound => NotFoundMessage,
                _ => "No se pudo procesar la solicitud."
            };
            return RedirectToAction(nameof(Index));
        }

        TempData["Mensaje"] = UpdateSuccessMessage;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleEstado(string id)
    {
        var usuario = await AdministradorService.GetStaffUserAsync(id, Rol);
        var seActivara = usuario is not null && !usuario.Activo;

        var resultado = await AdministradorService.ToggleStaffStatusAsync(id, CurrentUserId, Rol, AplicaAutoproteccion);

        TempData[resultado == StaffActionStatus.Success ? "Mensaje" : "Error"] = resultado switch
        {
            StaffActionStatus.Success => seActivara ? ActivateSuccessMessage : DeactivateSuccessMessage,
            StaffActionStatus.NotFound => NotFoundMessage,
            StaffActionStatus.CannotModifySelf => CannotDeactivateSelfMessage,
            StaffActionStatus.LastActiveAdmin => LastActiveMessage,
            _ => "No se pudo procesar la solicitud."
        };

        return RedirectToAction(nameof(Index));
    }

    private void CargarViewData()
    {
        ViewData["Titulo"] = Titulo;
        ViewData["EtiquetaCrear"] = EtiquetaCrear;
        ViewData["EntidadSingular"] = EntidadSingular;
        ViewData["EmptyListMessage"] = EmptyListMessage;
    }
}
