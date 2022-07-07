using TP3VeilleSignalR.Data.Models;
using TP3VeilleSignalR.Hubs;

namespace TP3VeilleSignalR.Services;

public interface IConversationService
{
    public Task<bool> ExistsAsync(string conversationId);
    public Task<bool> AddUserToConversationAsync(string conversationId, string username);
    public Task<bool> RemoveUserFromConversationAsync(string conversationId, string username);
    public Task<Conversation> GetByIdAsync(string conversationId);
    public Task<bool> AddMessageToConversation(string conversationId, ChatMessage message);
}