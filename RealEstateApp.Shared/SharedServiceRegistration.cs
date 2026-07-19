using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Interfaces.Services;

namespace RealEstateApp.Shared;

public static class SharedServiceRegistration
{
    // webRootPath viene de builder.Environment.WebRootPath en el Program.cs de
    // cada host -- asi Shared (un classlib puro) no necesita depender de
    // IWebHostEnvironment del framework web.
    public static IServiceCollection AddSharedServices(this IServiceCollection services, IConfiguration configuration, string webRootPath)
    {
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddScoped<IEmailService, EmailService>();

        services.Configure<FileStorageSettings>(options => options.WebRootPath = webRootPath);
        services.AddScoped<IFileStorageService, FileStorageService>();

        return services;
    }
}
