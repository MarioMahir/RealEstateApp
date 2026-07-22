using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Interfaces.Services;

namespace RealEstateApp.Shared;

public static class SharedServiceRegistration
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services, IConfiguration configuration, string webRootPath)
    {
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddScoped<IEmailService, EmailService>();

        services.Configure<FileStorageSettings>(options => options.WebRootPath = webRootPath);
        services.AddScoped<IFileStorageService, FileStorageService>();

        return services;
    }
}
