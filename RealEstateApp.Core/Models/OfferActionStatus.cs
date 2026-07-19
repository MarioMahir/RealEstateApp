namespace RealEstateApp.Core.Models;

public enum OfferActionStatus
{
    Success,
    NotFound,
    NotOwnedByAgent,

    // La oferta ya fue Aceptada/Rechazada antes -- aceptar/rechazar solo aplica
    // a ofertas en estado Pendiente.
    NotPending
}
