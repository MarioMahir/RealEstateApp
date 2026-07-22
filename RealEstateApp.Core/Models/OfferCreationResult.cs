using RealEstateApp.Core.Entities;

namespace RealEstateApp.Core.Models;

public enum OfferCreationStatus
{
    Success,
    PropertyNotAvailable,
    DuplicatePending
}

public class OfferCreationResult
{
    public OfferCreationStatus Status { get; set; }
    public Offer? Offer { get; set; }
}
