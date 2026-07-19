namespace RealEstateApp.Core.Models;

public enum OfferActionStatus
{
    Success,
    NotFound,
    NotOwnedByAgent,

    // La oferta ya fue Aceptada/Rechazada antes -- aceptar/rechazar solo aplica
    // a ofertas en estado Pendiente.
    NotPending,

    // El id de oferta existe pero no pertenece a la propiedad indicada en la
    // solicitud (ids de oferta/propiedad que no coinciden).
    PropertyMismatch,

    // Solo aplica a Aceptar: la propiedad ya no esta Disponible.
    PropertyNotAvailable
}
