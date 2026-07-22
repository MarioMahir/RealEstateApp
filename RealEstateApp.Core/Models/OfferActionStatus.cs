namespace RealEstateApp.Core.Models;

public enum OfferActionStatus
{
    Success,
    NotFound,
    NotOwnedByAgent,

    NotPending,

    PropertyMismatch,

    PropertyNotAvailable
}
