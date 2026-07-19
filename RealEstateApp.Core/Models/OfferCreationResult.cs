using RealEstateApp.Core.Entities;

namespace RealEstateApp.Core.Models;

public enum OfferCreationStatus
{
    Success,
    PropertyNotAvailable,
    DuplicatePending
}

// Distingue por que una oferta no se pudo crear (propiedad ya no disponible vs.
// oferta Pendiente duplicada) para que el controlador devuelva el mensaje exacto
// de cada caso, igual que CredentialValidationResult para el login.
public class OfferCreationResult
{
    public OfferCreationStatus Status { get; set; }
    public Offer? Offer { get; set; }
}
