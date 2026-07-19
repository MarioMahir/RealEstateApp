using RealEstateApp.Infrastructure;
using RealEstateApp.Infrastructure.Seed;
using RealEstateApp.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSharedServices(builder.Configuration);

// AddIdentity (dentro de AddInfrastructure) ya deja el esquema de cookies de
// Identity como esquema de autenticacion por defecto -- exactamente lo que
// necesita la WebApp. No se toca AddAuthentication aqui a proposito.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.SlidingExpiration = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

await app.Services.SeedDefaultDataAsync();

app.Run();
