using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Constants;
using RealEstateApp.Core.DTOs.Property;
using RealEstateApp.Core.Interfaces.Services;

namespace RealEstateApp.WebAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = $"{Roles.Administrador},{Roles.Desarrollador}")]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyService _propertyService;
    private readonly IMapper _mapper;

    public PropertiesController(IPropertyService propertyService, IMapper mapper)
    {
        _propertyService = propertyService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var properties = await _propertyService.GetAllWithDetailsAsync();
        if (properties.Count == 0)
            return NoContent();

        return Ok(_mapper.Map<List<PropertyDto>>(properties));
    }

    // Sin restriccion de ruta {id:int} a proposito: con esa restriccion, un id
    // no numerico (ej. "abc") ni siquiera llega a este controlador -- el
    // routing de ASP.NET Core devuelve un 404 crudo antes de Authorize/accion,
    // en vez del 400 que pide el documento funcional para un id con formato
    // invalido.
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        if (!int.TryParse(id, out var parsedId))
            return BadRequest(new { message = "El id de la propiedad debe ser un número entero." });

        var property = await _propertyService.GetByIdWithDetailsAsync(parsedId);
        if (property is null)
            return NotFound(new { message = "La propiedad solicitada no existe." });

        return Ok(_mapper.Map<PropertyDto>(property));
    }

    [HttpGet("code/{codigo}")]
    public async Task<IActionResult> GetByCode(string codigo)
    {
        if (!Regex.IsMatch(codigo, "^[0-9]{6}$"))
            return BadRequest(new { message = "El código de propiedad debe tener exactamente 6 dígitos numéricos." });

        var property = await _propertyService.GetByCodeWithDetailsAsync(codigo);
        if (property is null)
            return NotFound(new { message = "No existe una propiedad registrada con el código enviado." });

        return Ok(_mapper.Map<PropertyDto>(property));
    }
}
