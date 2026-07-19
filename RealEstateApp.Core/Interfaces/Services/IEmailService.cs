namespace RealEstateApp.Core.Interfaces.Services;

// Contrato vive en Core; la implementacion (MailKit) vive en RealEstateApp.Shared,
// separada por completo de la logica de persistencia de Infrastructure.
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlBody);
}
