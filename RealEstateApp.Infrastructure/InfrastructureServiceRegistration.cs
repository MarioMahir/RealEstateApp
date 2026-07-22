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
        services.AddScoped<IAdministradorService, AdministradorService>();
        services.AddScoped<IFavoriteService, FavoriteService>();
        services.AddScoped<IMessageService, MessageService>();

        services.AddAutoMapper(typeof(Core.Mappings.WebApiMappingProfile).Assembly);

        return services;
    }
}
