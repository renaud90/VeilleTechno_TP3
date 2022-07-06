using TP3VeilleSignalR.Data.Models;
using TP3VeilleSignalR.Hubs;

namespace TP3VeilleSignalR.Services;

public interface IConversationService
{
    public Task<bool> ConversationExists(string conversationId);
    public Task<bool> AddUserToConversation(string conversationId, string username);
    public Task<bool> RemoveUserFromConversation(string conversationId, string username);
    public Task<Conversation> GetConversationById(string conversationId);
    public Task<bool> AddMessageToConversation(string conversationId, ChatMessage message);
}