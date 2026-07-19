using RealEstateApp.Core.Entities;
using RealEstateApp.Core.Enums;
using RealEstateApp.Core.Models;

namespace RealEstateApp.Core.Interfaces.Services;

// Una conversacion es el conjunto de Message que comparten (Cliente, Agente,
// Propiedad) -- no existe una entidad "Conversacion" separada. Rol-agnostico a
// proposito: tanto el Cliente (Etapa 3) como el Agente (Etapa 4) envian
// mensajes a traves del mismo metodo, solo cambia el MessageSender.
public interface IMessageService
{
    Task<List<Message>> GetConversationAsync(string clienteId, string agenteId, int propertyId);

    Task<MessageSendResult> SendMessageAsync(
        string clienteId, string agenteId, int propertyId, MessageSender remitente, string texto);

    // Todos los mensajes de una propiedad (cualquier cliente) -- Detalle de
    // propiedad (agente), agrupados por cliente en el controlador/vista.
    Task<List<Message>> GetByPropertyAsync(int propertyId);
}
