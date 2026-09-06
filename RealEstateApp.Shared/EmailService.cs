using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using RealEstateApp.Core.Interfaces.Services;

namespace RealEstateApp.Shared;

/// <summary>
/// Servicio de correo de la capa Shared. Con SMTP configurado envía por MailKit;
/// sin usuario configurado, guarda cada correo como archivo HTML para poder probar
/// el registro y la activación de clientes en desarrollo sin credenciales.
/// </summary>
public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        if (!_settings.IsSmtpConfigured)
        {
            await SaveToFileAsync(to, subject, htmlBody);
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.DisplayName, _settings.From));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_settings.User, _settings.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);

        _logger.LogInformation("Correo enviado a {To}: {Subject}", to, subject);
    }

    private async Task SaveToFileAsync(string to, string subject, string htmlBody)
    {
        var folder = Path.Combine(Directory.GetCurrentDirectory(), _settings.OutputFolder);
        Directory.CreateDirectory(folder);

        var safeTo = string.Concat(to.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
        var path = Path.Combine(folder, $"{DateTime.Now:yyyyMMdd-HHmmss-fff}_{safeTo}.html");

        var html = $"""
            <!DOCTYPE html>
            <html lang="es"><head><meta charset="utf-8"><title>{subject}</title></head>
            <body style="font-family:Segoe UI,Arial,sans-serif;max-width:640px;margin:24px auto;padding:0 16px">
            <div style="background:#f1f3f5;border-radius:8px;padding:12px 16px;font-size:13px;color:#495057">
              <strong>Correo simulado (sin SMTP configurado)</strong><br>
              Para: {to}<br>
              Asunto: {subject}<br>
              Fecha: {DateTime.Now:dd/MM/yyyy HH:mm:ss}
            </div>
            <hr>
            {htmlBody}
            </body></html>
            """;

        await File.WriteAllTextAsync(path, html);

        _logger.LogWarning("SMTP no configurado. Correo para {To} guardado en {Path}", to, path);
    }
}
