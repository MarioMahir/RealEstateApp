using RealEstateApp.Core.Entities;

namespace RealEstateApp.Core.Models;

public enum CredentialValidationStatus
{
    Success,
    InvalidCredentials,
    Inactive
}

// Distingue explicitamente "credenciales invalidas" de "usuario inactivo" para
// que el controlador pueda devolver el mensaje exacto de cada caso (ambos son
// 401, pero con texto distinto segun el documento funcional).
public class CredentialValidationResult
{
    public CredentialValidationStatus Status { get; set; }
    public ApplicationUser? User { get; set; }
}
