using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RealEstateApp.Infrastructure;
using RealEstateApp.Infrastructure.Seed;
using RealEstateApp.Shared;
using RealEstateApp.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSharedServices(builder.Configuration, builder.Environment.WebRootPath);
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// AddIdentity (dentro de AddInfrastructure) deja el esquema de cookies de
// Identity como esquema por defecto. La WebAPI lo sobrescribe explicitamente a
// JWT: sin esto, Forbid()/Unauthorized() en los controladores heredarian
// semantica de cookie (redirects) en vez de 401/403 crudos, rompiendo la regla
// de login cruzado (401 credenciales invalidas/usuario inactivo, 403 rol no
// autorizado).
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };

    // Sin esto, un token ausente/invalido o un rol no autorizado producen un
    // 401/403 con cuerpo vacio (comportamiento por defecto de JwtBearer) --
    // la rubrica pide un mensaje claro en el cuerpo de ambas respuestas.
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsJsonAsync(new { message = "No está autorizado para acceder a este recurso." });
        },
        OnForbidden = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsJsonAsync(new { message = "Acceso denegado. No tiene permisos para realizar esta acción." });
        }
    };
});
builder.Services.AddAuthorization();

builder.Services.AddControllers();

// Unifica la forma de las respuestas de error: sin esto, un DataAnnotation
// fallido (ej. [Required] en un DTO) responde con el ValidationProblemDetails
// de ASP.NET Core ({"type","title","status","errors":{...}}), mientras que
// las validaciones de negocio ya escritas a mano en los controladores
// responden con {"message": "..."} -- dos formas distintas de error 400
// conviviendo en la misma API. Esto reescribe el 400 automatico para que use
// la MISMA forma que el resto de la API.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var mensaje = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .FirstOrDefault(m => !string.IsNullOrWhiteSpace(m))
            ?? "Los datos enviados no son válidos.";

        return new BadRequestObjectResult(new { message = mensaje });
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "RealEstateApp.WebAPI", Version = "v1" });

    var bearerScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pega el token JWT obtenido en /api/v1/Account/Login (sin la palabra 'Bearer').",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    options.AddSecurityDefinition("Bearer", bearerScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { bearerScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.Services.SeedDefaultDataAsync();

app.Run();
