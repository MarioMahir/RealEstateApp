using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Constants;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Enums;
using RealEstateApp.Core.Interfaces.Services;
using RealEstateApp.Core.Models;
using RealEstateApp.Core.ViewModels.Agente;
using RealEstateApp.Core.ViewModels.Cliente;
using RealEstateApp.Core.ViewModels.Property;

namespace RealEstateApp.WebApp.Controllers;

[Authorize(Roles = Roles.Agente)]
public class AgenteController : Controller
{
    private readonly IPropertyService _propertyService;
    private readonly IMessageService _messageService;
    private readonly IOfferService _offerService;
    private readonly IFileStorageService _fileStorageService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public AgenteController(
        IPropertyService propertyService,
        IMessageService messageService,
        IOfferService offerService,
        IFileStorageService fileStorageService,
        UserManager<ApplicationUser> userManager,
        IMapper mapper)
    {
        _propertyService = propertyService;
        _messageService = messageService;
        _offerService = offerService;
        _fileStorageService = fileStorageService;
        _userManager = userManager;
        _mapper = mapper;
    }

    private string AgentId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public async Task<IActionResult> Index()
    {
        var propiedades = await _propertyService.GetAllByAgentAsync(AgentId);

        var modelo = _mapper.Map<List<PropertyListItemViewModel>>(propiedades);
        if (modelo.Count == 0)
            ViewData["Mensaje"] = "No tiene propiedades registradas en este momento.";

        return View(modelo);
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(int id, string? clienteId)
    {
        var propiedad = await _propertyService.GetByIdForAgentAsync(id, AgentId);
        if (propiedad is null)
        {
            TempData["Error"] = "La propiedad solicitada no existe.";
            return RedirectToAction(nameof(Index));
        }

        return View(await ConstruirDetalleAsync(propiedad, clienteId));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Responder(AgentReplyViewModel modelo)
    {
        var propiedad = await _propertyService.GetByIdForAgentAsync(modelo.PropertyId, AgentId);
        if (propiedad is null)
            return RedirectToAction(nameof(Index));

        if (!ModelState.IsValid)
            return View(nameof(Detalle), await ConstruirDetalleAsync(propiedad, modelo.ClienteId, modelo));

        var mensajesPrevios = await _messageService.GetByPropertyAsync(propiedad.Id);
        if (!mensajesPrevios.Any(m => m.ClienteId == modelo.ClienteId))
        {
            TempData["Error"] = "El cliente seleccionado no tiene una conversación asociada a esta propiedad.";
            return RedirectToAction(nameof(Detalle), new { id = propiedad.Id });
        }

        var resultado = await _messageService.SendMessageAsync(
            modelo.ClienteId, AgentId, propiedad.Id, MessageSender.Agente, modelo.Texto);

        TempData[resultado.Status == MessageSendStatus.Success ? "Mensaje" : "Error"] =
            resultado.Status == MessageSendStatus.Success
                ? "Mensaje enviado correctamente."
                : "Esta propiedad ya no admite mensajes nuevos.";

        return RedirectToAction(nameof(Detalle), new { id = propiedad.Id, clienteId = modelo.ClienteId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AceptarOferta(int offerId, int propertyId)
    {
        var resultado = await _offerService.AcceptOfferAsync(offerId, AgentId, propertyId);
        TempData[resultado == OfferActionStatus.Success ? "Mensaje" : "Error"] = resultado switch
        {
            OfferActionStatus.Success => "La oferta fue aceptada correctamente y la propiedad fue marcada como vendida.",
            OfferActionStatus.NotFound => "La oferta solicitada no existe.",
            OfferActionStatus.PropertyMismatch => "La oferta solicitada no existe.",
            OfferActionStatus.NotOwnedByAgent => "No tiene permisos sobre esta oferta.",
            OfferActionStatus.PropertyNotAvailable => "No se puede aceptar una oferta para una propiedad que ya fue vendida.",
            OfferActionStatus.NotPending => "Esta oferta ya fue respondida.",
            _ => "No se pudo procesar la solicitud."
        };

        return RedirectToAction(nameof(Detalle), new { id = propertyId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RechazarOferta(int offerId, int propertyId)
    {
        var resultado = await _offerService.RejectOfferAsync(offerId, AgentId, propertyId);
        TempData[resultado == OfferActionStatus.Success ? "Mensaje" : "Error"] = resultado switch
        {
            OfferActionStatus.Success => "La oferta fue rechazada correctamente.",
            OfferActionStatus.NotFound => "La oferta solicitada no existe.",
            OfferActionStatus.PropertyMismatch => "La oferta solicitada no existe.",
            OfferActionStatus.NotOwnedByAgent => "No tiene permisos sobre esta oferta.",
            OfferActionStatus.NotPending => "Esta oferta ya fue respondida.",
            _ => "No se pudo procesar la solicitud."
        };

        return RedirectToAction(nameof(Detalle), new { id = propertyId });
    }

    [HttpGet]
    public async Task<IActionResult> Perfil()
    {
        var usuario = await _userManager.GetUserAsync(User);
        return View(new ProfileViewModel
        {
            Nombre = usuario!.Nombre,
            Apellido = usuario.Apellido,
            Telefono = usuario.PhoneNumber ?? string.Empty,
            FotoUrl = usuario.FotoUrl
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Perfil(ProfileViewModel modelo, IFormFile? foto)
    {
        var usuario = await _userManager.GetUserAsync(User);

        if (!ModelState.IsValid)
        {
            modelo.FotoUrl = usuario!.FotoUrl;
            return View(modelo);
        }

        if (foto is { Length: > 0 })
        {
            var fotoAnterior = usuario!.FotoUrl;
            try
            {
                usuario.FotoUrl = await _fileStorageService.SaveImageAsync(foto.OpenReadStream(), foto.FileName, "usuarios");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                modelo.FotoUrl = usuario.FotoUrl;
                return View(modelo);
            }

            if (!string.IsNullOrEmpty(fotoAnterior))
                _fileStorageService.DeleteImage(fotoAnterior);
        }

        usuario!.Nombre = modelo.Nombre;
        usuario.Apellido = modelo.Apellido;
        usuario.PhoneNumber = modelo.Telefono;
        await _userManager.UpdateAsync(usuario);

        TempData["Mensaje"] = "Su perfil fue actualizado correctamente.";
        return RedirectToAction(nameof(Perfil));
    }

    private async Task<AgentPropertyDetailViewModel> ConstruirDetalleAsync(
        Property propiedad, string? clienteId, AgentReplyViewModel? respuestaFallida = null)
    {
        var mensajes = await _messageService.GetByPropertyAsync(propiedad.Id);
        var ofertas = await _offerService.GetByPropertyAsync(propiedad.Id);

        var conversaciones = mensajes
            .GroupBy(m => m.ClienteId)
            .Select(g => new ConversationSummaryViewModel
            {
                ClienteId = g.Key,
                NombreCliente = $"{g.First().Cliente.Nombre} {g.First().Cliente.Apellido}",
                UltimoMensajeFecha = g.Max(m => m.Fecha),
                UltimoMensajeTexto = g.OrderByDescending(m => m.Fecha).First().Texto
            })
            .OrderByDescending(c => c.UltimoMensajeFecha)
            .ToList();

        var gruposDeOfertas = ofertas
            .GroupBy(o => o.ClienteId)
            .Select(g => new OfferGroupViewModel
            {
                ClienteId = g.Key,
                NombreCliente = $"{g.First().Cliente.Nombre} {g.First().Cliente.Apellido}",
                Ofertas = _mapper.Map<List<OfferItemViewModel>>(g.OrderByDescending(o => o.Fecha).ToList())
            })
            .ToList();

        var modelo = new AgentPropertyDetailViewModel
        {
            Propiedad = _mapper.Map<PropertyDetailViewModel>(propiedad),
            Conversaciones = conversaciones,
            GruposDeOfertas = gruposDeOfertas
        };

        if (!string.IsNullOrEmpty(clienteId))
        {
            var hilo = mensajes.Where(m => m.ClienteId == clienteId).OrderBy(m => m.Fecha).ToList();
            if (hilo.Count > 0)
            {
                modelo.ClienteSeleccionadoId = clienteId;
                modelo.ClienteSeleccionadoNombre = $"{hilo[0].Cliente.Nombre} {hilo[0].Cliente.Apellido}";
                modelo.HiloSeleccionado = _mapper.Map<List<MessageItemViewModel>>(hilo);
            }
        }

        modelo.Respuesta = respuestaFallida ?? new AgentReplyViewModel { PropertyId = propiedad.Id, ClienteId = clienteId ?? string.Empty };

        return modelo;
    }
}
