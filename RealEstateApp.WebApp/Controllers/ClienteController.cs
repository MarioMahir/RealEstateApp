using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Constants;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Enums;
using RealEstateApp.Core.Interfaces.Services;
using RealEstateApp.Core.Models;
using RealEstateApp.Core.ViewModels.Cliente;
using RealEstateApp.Core.ViewModels.Property;

namespace RealEstateApp.WebApp.Controllers;

[Authorize(Roles = Roles.Cliente)]
public class ClienteController : Controller
{
    private readonly IPropertyService _propertyService;
    private readonly IFavoriteService _favoriteService;
    private readonly IOfferService _offerService;
    private readonly IMessageService _messageService;
    private readonly IMapper _mapper;

    public ClienteController(
        IPropertyService propertyService,
        IFavoriteService favoriteService,
        IOfferService offerService,
        IMessageService messageService,
        IMapper mapper)
    {
        _propertyService = propertyService;
        _favoriteService = favoriteService;
        _offerService = offerService;
        _messageService = messageService;
        _mapper = mapper;
    }

    private string ClienteId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // "Mis propiedades" = favoritos que siguen Disponible.
    public async Task<IActionResult> MisPropiedades()
    {
        var propiedades = await _propertyService.GetFavoritesByClienteAsync(ClienteId);
        var items = _mapper.Map<List<PropertyListItemViewModel>>(propiedades);
        items.ForEach(p => p.EsFavorito = true);

        ViewData["ContextoCliente"] = true;

        var modelo = new PropertyBrowseViewModel
        {
            Propiedades = items,
            Mensaje = items.Count == 0 ? "Todavía no tienes propiedades favoritas disponibles." : null
        };

        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleFavorito(int propertyId, string? returnUrl)
    {
        var resultado = await _favoriteService.ToggleFavoriteAsync(ClienteId, propertyId);

        TempData[resultado is null ? "Error" : "Mensaje"] = resultado switch
        {
            true => "La propiedad fue agregada a tus favoritos.",
            false => "La propiedad fue eliminada de tus favoritos.",
            null => "La propiedad solicitada no existe."
        };

        return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? Redirect(returnUrl)
            : RedirectToAction(nameof(HomeController.Index), "Home");
    }

    // Detalle de propiedad (cliente) = Detalle publico + chat con el agente +
    // mis ofertas sobre esa propiedad.
    public async Task<IActionResult> Detalle(int id)
    {
        var propiedad = await _propertyService.GetByIdWithDetailsAsync(id);
        if (propiedad is null)
        {
            TempData["Error"] = "La propiedad solicitada no existe.";
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        var modelo = await ConstruirDetalleAsync(propiedad);

        // Una propiedad Vendida desaparece del Home/Mis propiedades, pero el
        // historial de ofertas/mensajes propio "nunca se borra" -- si el cliente
        // ya tenia alguna interaccion aqui, sigue pudiendo consultarla.
        var accesible = propiedad.Estado == PropertyStatus.Disponible
            || modelo.MisOfertas.Count > 0
            || modelo.Mensajes.Count > 0;
        if (!accesible)
        {
            TempData["Error"] = "La propiedad solicitada no existe o ya no se encuentra disponible.";
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnviarMensaje(SendMessageViewModel modelo)
    {
        var propiedad = await _propertyService.GetByIdWithDetailsAsync(modelo.PropertyId);
        if (propiedad is null)
            return RedirectToAction(nameof(HomeController.Index), "Home");

        if (!ModelState.IsValid)
            return View(nameof(Detalle), await ConstruirDetalleAsync(propiedad, mensajeFallido: modelo));

        var resultado = await _messageService.SendMessageAsync(
            ClienteId, propiedad.AgentId, propiedad.Id, MessageSender.Cliente, modelo.Texto);

        TempData[resultado.Status == MessageSendStatus.Success ? "Mensaje" : "Error"] =
            resultado.Status == MessageSendStatus.Success
                ? "Mensaje enviado correctamente."
                : "Esta propiedad ya no admite mensajes nuevos.";

        return RedirectToAction(nameof(Detalle), new { id = propiedad.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearOferta(CreateOfferViewModel modelo)
    {
        var propiedad = await _propertyService.GetByIdWithDetailsAsync(modelo.PropertyId);
        if (propiedad is null)
            return RedirectToAction(nameof(HomeController.Index), "Home");

        if (!ModelState.IsValid)
            return View(nameof(Detalle), await ConstruirDetalleAsync(propiedad, ofertaFallida: modelo));

        var resultado = await _offerService.CreateOfferAsync(ClienteId, propiedad.Id, modelo.Monto);

        TempData[resultado.Status == OfferCreationStatus.Success ? "Mensaje" : "Error"] = resultado.Status switch
        {
            OfferCreationStatus.Success => "Su oferta fue enviada correctamente.",
            OfferCreationStatus.DuplicatePending => "Ya tiene una oferta pendiente sobre esta propiedad.",
            OfferCreationStatus.PropertyNotAvailable => "Esta propiedad ya no admite ofertas nuevas.",
            _ => "No se pudo procesar la oferta."
        };

        return RedirectToAction(nameof(Detalle), new { id = propiedad.Id });
    }

    private async Task<ClientPropertyDetailViewModel> ConstruirDetalleAsync(
        Property propiedad, SendMessageViewModel? mensajeFallido = null, CreateOfferViewModel? ofertaFallida = null)
    {
        var ofertas = await _offerService.GetByClienteAndPropertyAsync(ClienteId, propiedad.Id);
        var mensajes = await _messageService.GetConversationAsync(ClienteId, propiedad.AgentId, propiedad.Id);

        return new ClientPropertyDetailViewModel
        {
            Propiedad = _mapper.Map<PropertyDetailViewModel>(propiedad),
            MisOfertas = _mapper.Map<List<OfferItemViewModel>>(ofertas),
            Mensajes = _mapper.Map<List<MessageItemViewModel>>(mensajes),
            PermiteNuevaOferta = propiedad.Estado == PropertyStatus.Disponible
                && !ofertas.Any(o => o.Estado == OfferStatus.Pendiente),
            NuevoMensaje = mensajeFallido ?? new SendMessageViewModel { PropertyId = propiedad.Id },
            NuevaOferta = ofertaFallida ?? new CreateOfferViewModel { PropertyId = propiedad.Id }
        };
    }
}
