using RealEstateApp.Core.Entities;

namespace RealEstateApp.Core.Models;

public enum MessageSendStatus
{
    Success,
    PropertyNotAvailable
}

public class MessageSendResult
{
    public MessageSendStatus Status { get; set; }
    public Message? Message { get; set; }
}
