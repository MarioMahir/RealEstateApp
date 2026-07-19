using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Interfaces.Repositories;
using RealEstateApp.Core.Interfaces.Services;
using RealEstateApp.Infrastructure.Persistence;
using RealEstateApp.Infrastructure.Repositories;
using RealEstateApp.Infrastructure.Services;

namespace RealEstateApp.Infrastructure;

// Unico punto de registro de DI para persistencia/Identity/repos/servicios.
// Tanto WebApp como WebAPI llaman a este mismo metodo desde su Program.cs,
// apuntando a la misma cadena de conexion -- asi comparten el mismo esquema de
// usuarios/roles sin compartir proceso.
public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));

        services.AddScoped<IOfferService, OfferService>();
        services.AddScoped<IPropertyService, PropertyService>();
        services.AddScoped<IAgentService, AgentService>();
        services.AddScoped<IAccountService, AccountService>();

        // Un solo registro, apuntando al ensamblado de Core: carga todos los
        // Profile que haya ahi (WebApiMappingProfile hoy; WebAppMappingProfile
        // se suma en una etapa futura sin tocar este archivo).
        services.AddAutoMapper(typeof(Core.Mappings.WebApiMappingProfile).Assembly);

        return services;
    }
}
