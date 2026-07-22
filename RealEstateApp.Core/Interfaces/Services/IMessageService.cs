using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Enums;
using RealEstateApp.Core.Models;

namespace RealEstateApp.Core.Interfaces.Services;

public interface IMessageService
{
    Task<List<Message>> GetConversationAsync(string clienteId, string agenteId, int propertyId);

    Task<MessageSendResult> SendMessageAsync(
        string clienteId, string agenteId, int propertyId, MessageSender remitente, string texto);

    Task<List<Message>> GetByPropertyAsync(int propertyId);
}
