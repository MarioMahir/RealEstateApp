using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Constants;
using RealEstateApp.Core.DTOs.Property;
using RealEstateApp.Core.Interfaces.Services;

namespace RealEstateApp.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var property = await _propertyService.GetByIdWithDetailsAsync(id);
        if (property is null)
            return NotFound(new { message = "La propiedad solicitada no existe." });

        return Ok(_mapper.Map<PropertyDto>(property));
    }

    [HttpGet("code/{codigo}")]
    public async Task<IActionResult> GetByCode(string codigo)
    {
        var property = await _propertyService.GetByCodeWithDetailsAsync(codigo);
        if (property is null)
            return NotFound(new { message = "No existe una propiedad registrada con el código enviado." });

        return Ok(_mapper.Map<PropertyDto>(property));
    }
}
