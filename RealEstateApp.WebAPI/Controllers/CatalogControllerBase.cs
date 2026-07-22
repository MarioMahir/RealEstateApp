using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Constants;
using RealEstateApp.Core.DTOs.Catalog;
using RealEstateApp.Core.Interfaces.Entities;
using RealEstateApp.Core.Interfaces.Services;

namespace RealEstateApp.WebAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class CatalogControllerBase<TEntity> : ControllerBase where TEntity : class, ICatalogItem
{
    protected readonly IGenericService<TEntity> Service;
    protected readonly IMapper Mapper;

    protected CatalogControllerBase(IGenericService<TEntity> service, IMapper mapper)
    {
        Service = service;
        Mapper = mapper;
    }

    protected abstract string NotFoundMessage { get; }
    protected abstract string DuplicateNameMessage { get; }
    protected abstract string DuplicateNameOnUpdateMessage { get; }

    [HttpGet]
    [Authorize(Roles = $"{Roles.Administrador},{Roles.Desarrollador}")]
    public async Task<IActionResult> List()
    {
        var items = await Service.GetAllAsync();
        return items.Count == 0 ? NoContent() : Ok(Mapper.Map<List<CatalogItemDto>>(items));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = $"{Roles.Administrador},{Roles.Desarrollador}")]
    public async Task<IActionResult> GetById(string id)
    {
        if (!int.TryParse(id, out var parsedId))
            return BadRequest(new { message = "El id debe ser un número entero." });

        var item = await Service.GetByIdAsync(parsedId);
        return item is null ? NotFound(new { message = NotFoundMessage }) : Ok(Mapper.Map<CatalogItemDto>(item));
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<IActionResult> Create(CatalogItemUpsertDto dto)
    {
        var nombre = dto.Nombre.Trim();
        if (nombre.Length == 0)
            return BadRequest(new { message = "Los datos enviados no son válidos." });

        var existentes = await Service.GetAllAsync();
        if (existentes.Any(x => x.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
            return BadRequest(new { message = DuplicateNameMessage });

        var entity = Mapper.Map<TEntity>(dto);
        entity.Nombre = nombre;
        await Service.AddAsync(entity);

        return StatusCode(StatusCodes.Status201Created, Mapper.Map<CatalogItemDto>(entity));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<IActionResult> Update(string id, CatalogItemUpsertDto dto)
    {
        if (!int.TryParse(id, out var parsedId))
            return BadRequest(new { message = "El id debe ser un número entero." });

        var entity = await Service.GetByIdAsync(parsedId);
        if (entity is null)
            return NotFound(new { message = NotFoundMessage });

        var nombre = dto.Nombre.Trim();
        if (nombre.Length == 0)
            return BadRequest(new { message = "Los datos enviados no son válidos." });

        var existentes = await Service.GetAllAsync();
        if (existentes.Any(x => x.Id != parsedId && x.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
            return BadRequest(new { message = DuplicateNameOnUpdateMessage });

        entity.Nombre = nombre;
        entity.Descripcion = dto.Descripcion;
        await Service.UpdateAsync(entity);

        return Ok(Mapper.Map<CatalogItemDto>(entity));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<IActionResult> Delete(string id)
    {
        if (!int.TryParse(id, out var parsedId))
            return BadRequest(new { message = "El id debe ser un número entero." });

        var entity = await Service.GetByIdAsync(parsedId);
        if (entity is null)
            return NotFound(new { message = NotFoundMessage });

        await Service.DeleteAsync(entity);
        return NoContent();
    }
}
