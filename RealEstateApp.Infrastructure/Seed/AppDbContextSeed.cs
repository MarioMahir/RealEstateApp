using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Constants;
using RealEstateApp.Core.Entities;
using RealEstateApp.Infrastructure.Persistence;

namespace RealEstateApp.Infrastructure.Seed;

public static class AppDbContextSeed
{
    // Idempotente a proposito: seguro de invocar desde el arranque de la WebApp
    // y de la WebAPI, sin importar cual de las dos corra primero.
    public static async Task SeedDefaultDataAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
        var context = provider.GetRequiredService<AppDbContext>();

        await context.Database.MigrateAsync();

        foreach (var role in Core.Constants.Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        await CrearUsuarioSiNoExisteAsync(userManager, "admin@realestateapp.com", "Admin123$",
            Core.Constants.Roles.Administrador, "Admin", "Principal", cedula: "00100000001");
        await CrearUsuarioSiNoExisteAsync(userManager, "desarrollador@realestateapp.com", "Developer123$",
            Core.Constants.Roles.Desarrollador, "Dev", "Principal", cedula: "00100000002");
        await CrearUsuarioSiNoExisteAsync(userManager, "cliente@realestateapp.com", "Cliente123$",
            Core.Constants.Roles.Cliente, "Cliente", "Demo");
        await CrearUsuarioSiNoExisteAsync(userManager, "agente@realestateapp.com", "Agente123$",
            Core.Constants.Roles.Agente, "Agente", "Demo");

        // Catalogo minimo para que el alta de propiedades (Etapa 4) sea probable
        // desde la primera corrida. Nombres tomados literalmente de los ejemplos
        // del documento funcional, no inventados.
        if (!context.PropertyTypes.Any())
        {
            context.PropertyTypes.AddRange(
                new PropertyType { Nombre = "Casa", Descripcion = "Vivienda unifamiliar." },
                new PropertyType { Nombre = "Apartamento", Descripcion = "Unidad habitacional dentro de un edificio." },
                new PropertyType { Nombre = "Villa", Descripcion = "Residencia de mayor tamano, usualmente con areas verdes." },
                new PropertyType { Nombre = "Solar", Descripcion = "Terreno sin construir." },
                new PropertyType { Nombre = "Local comercial", Descripcion = "Espacio destinado a actividades comerciales." });
        }

        if (!context.SaleTypes.Any())
        {
            context.SaleTypes.AddRange(
                new SaleType { Nombre = "Venta", Descripcion = "Transferencia definitiva de la propiedad." },
                new SaleType { Nombre = "Alquiler", Descripcion = "Uso temporal de la propiedad a cambio de un pago periodico." },
                new SaleType { Nombre = "Alquiler con opcion a compra", Descripcion = "Alquiler que incluye la opcion de comprar la propiedad mas adelante." });
        }

        if (!context.Improvements.Any())
        {
            context.Improvements.AddRange(
                new Improvement { Nombre = "Piscina", Descripcion = "Piscina disponible en la propiedad." },
                new Improvement { Nombre = "Terraza", Descripcion = "Area de terraza." },
                new Improvement { Nombre = "Marquesina", Descripcion = "Espacio techado para estacionar vehiculos." },
                new Improvement { Nombre = "Seguridad 24 horas", Descripcion = "Vigilancia permanente." },
                new Improvement { Nombre = "Ascensor", Descripcion = "Ascensor disponible en el edificio." },
                new Improvement { Nombre = "Planta electrica", Descripcion = "Planta electrica de respaldo." },
                new Improvement { Nombre = "Area de lavado", Descripcion = "Espacio destinado para lavado de ropa." });
        }

        await context.SaveChangesAsync();
    }

    private static async Task CrearUsuarioSiNoExisteAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string role,
        string nombre,
        string apellido,
        string? cedula = null)
    {
        if (await userManager.FindByEmailAsync(email) is not null)
            return;

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            Nombre = nombre,
            Apellido = apellido,
            Cedula = cedula,
            Activo = true
        };

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(user, role);
    }
}
