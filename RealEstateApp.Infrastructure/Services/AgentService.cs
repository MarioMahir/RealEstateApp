using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Constants;
using RealEstateApp.Core.DTOs.Agent;
using RealEstateApp.Core.DTOs.Property;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Interfaces.Repositories;
using RealEstateApp.Core.Interfaces.Services;

namespace RealEstateApp.Infrastructure.Services;

public class AgentService : IAgentService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorageService;

    public AgentService(
        UserManager<ApplicationUser> userManager,
        IGenericRepository<Property> propertyRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IFileStorageService fileStorageService)
    {
        _userManager = userManager;
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileStorageService = fileStorageService;
    }

    public async Task<List<AgentDto>> GetAllAsync()
    {
        var agentUsers = await _userManager.GetUsersInRoleAsync(Roles.Agente);
        var dtos = _mapper.Map<List<AgentDto>>(agentUsers);
        await AttachPropertyCountsAsync(dtos);
        return dtos;
    }

    public async Task<AgentDto?> GetByIdAsync(string id)
    {
        var user = await GetAgentUserOrNullAsync(id);
        if (user is null) return null;

        var dto = _mapper.Map<AgentDto>(user);
        dto.CantidadPropiedades = _propertyRepository.Query().Count(p => p.AgentId == id);
        return dto;
    }

    public async Task<List<PropertyDto>?> GetAgentPropertiesAsync(string agentId)
    {
        var user = await GetAgentUserOrNullAsync(agentId);
        if (user is null) return null;

        var properties = await _propertyRepository.Query()
            .Where(p => p.AgentId == agentId)
            .Include(p => p.PropertyType)
            .Include(p => p.SaleType)
            .Include(p => p.Agent)
            .Include(p => p.PropertyImprovements).ThenInclude(pi => pi.Improvement)
            .ToListAsync();

        return _mapper.Map<List<PropertyDto>>(properties);
    }

    public async Task<bool> ChangeStatusAsync(string agentId, bool activo)
    {
        var user = await GetAgentUserOrNullAsync(agentId);
        if (user is null) return false;

        user.Activo = activo;
        await _userManager.UpdateAsync(user);
        return true;
    }

    public async Task<List<ApplicationUser>> GetActiveAgentsAsync(string? nombreBusqueda)
    {
        var agentes = await _userManager.GetUsersInRoleAsync(Roles.Agente);
        var query = agentes.Where(a => a.Activo);

        if (!string.IsNullOrWhiteSpace(nombreBusqueda))
        {
            var texto = nombreBusqueda.Trim();
            query = query.Where(a =>
                a.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
                a.Apellido.Contains(texto, StringComparison.OrdinalIgnoreCase));
        }

        return query.OrderBy(a => a.Nombre).ThenBy(a => a.Apellido).ToList();
    }

    public async Task<ApplicationUser?> GetActiveAgentByIdAsync(string id)
    {
        var user = await GetAgentUserOrNullAsync(id);
        return user is { Activo: true } ? user : null;
    }

    public async Task<bool> DeleteAgentAsync(string agentId)
    {
        var user = await GetAgentUserOrNullAsync(agentId);
        if (user is null) return false;

        // Borra primero las Property del agente: eso SI cascada (a nivel de
        // FK real) imagenes/ofertas/mensajes/favoritos/mejoras. El FK
        // Agente->Property es Restrict a proposito (ver Modelo de dominio en
        // la guia del proyecto), asi que el ApplicationUser solo puede borrarse despues.
        var properties = await _propertyRepository.Query()
            .Where(p => p.AgentId == agentId)
            .Include(p => p.Images)
            .ToListAsync();
        var imagenesAEliminar = properties.SelectMany(p => p.Images.Select(i => i.Url)).ToList();

        foreach (var property in properties)
            _propertyRepository.Delete(property);
        await _unitOfWork.SaveChangesAsync();

        var resultado = await _userManager.DeleteAsync(user);
        if (!resultado.Succeeded) return false;

        foreach (var url in imagenesAEliminar)
            _fileStorageService.DeleteImage(url);
        if (!string.IsNullOrEmpty(user.FotoUrl))
            _fileStorageService.DeleteImage(user.FotoUrl);

        return true;
    }

    private async Task<ApplicationUser?> GetAgentUserOrNullAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return null;
        return await _userManager.IsInRoleAsync(user, Roles.Agente) ? user : null;
    }

    private async Task AttachPropertyCountsAsync(List<AgentDto> dtos)
    {
        var counts = await _propertyRepository.Query()
            .GroupBy(p => p.AgentId)
            .Select(g => new { AgentId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.AgentId, x => x.Count);

        foreach (var dto in dtos)
            dto.CantidadPropiedades = counts.GetValueOrDefault(dto.Id);
    }
}
