using RealEstateApp.Core.Entities;

namespace RealEstateApp.Core.Models;

public enum CredentialValidationStatus
{
    Success,
    InvalidCredentials,
    Inactive
}

public class CredentialValidationResult
{
    public CredentialValidationStatus Status { get; set; }
    public ApplicationUser? User { get; set; }
}
