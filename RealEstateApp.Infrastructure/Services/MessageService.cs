using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Enums;
using RealEstateApp.Core.Interfaces.Repositories;
using RealEstateApp.Core.Interfaces.Services;
using RealEstateApp.Core.Models;

namespace RealEstateApp.Infrastructure.Services;

public class MessageService : IMessageService
{
    private readonly IGenericRepository<Message> _messageRepository;
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MessageService(
        IGenericRepository<Message> messageRepository,
        IGenericRepository<Property> propertyRepository,
        IUnitOfWork unitOfWork)
    {
        _messageRepository = messageRepository;
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
    }

    public Task<List<Message>> GetConversationAsync(string clienteId, string agenteId, int propertyId) =>
        _messageRepository.Query()
            .Where(m => m.ClienteId == clienteId && m.AgenteId == agenteId && m.PropertyId == propertyId)
            .OrderBy(m => m.Fecha)
            .ToListAsync();

    public async Task<MessageSendResult> SendMessageAsync(
        string clienteId, string agenteId, int propertyId, MessageSender remitente, string texto)
    {
        var property = await _propertyRepository.GetByIdAsync(propertyId);
        if (property is null || property.Estado != PropertyStatus.Disponible)
            return new MessageSendResult { Status = MessageSendStatus.PropertyNotAvailable };

        var message = new Message
        {
            ClienteId = clienteId,
            AgenteId = agenteId,
            PropertyId = propertyId,
            Remitente = remitente,
            Texto = texto,
            Fecha = DateTime.Now
        };

        await _messageRepository.AddAsync(message);
        await _unitOfWork.SaveChangesAsync();

        return new MessageSendResult { Status = MessageSendStatus.Success, Message = message };
    }

    public Task<List<Message>> GetByPropertyAsync(int propertyId) =>
        _messageRepository.Query()
            .Where(m => m.PropertyId == propertyId)
            .Include(m => m.Cliente)
            .OrderBy(m => m.Fecha)
            .ToListAsync();
}
