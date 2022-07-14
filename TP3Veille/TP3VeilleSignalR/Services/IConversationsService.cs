using TP3VeilleSignalR.Data.Models;

namespace TP3VeilleSignalR.Services;

public interface IConversationsService
{
    public Task<bool> ExistsAsync(string conversationId);
    public Task<Conversation?> GetByIdAsync(string conversationId);
    public Task<IEnumerable<Message>> GetMessagesForConversationAsync(string conversationId);
    public Task<IEnumerable<Message>> GetMessagesForConversationAsync(string conversationId, Func<Message, bool> predicate);
    public Task<UserData> GetConversationsDataForUser(string userId);
    public Task<Conversation?> CreateAsync(string conversationId);
    public Task<bool> AddUserToConversationAsync(string conversationId, string username);
    public Task<bool> AddMessageToConversationAsync(string conversationId, Message message);
    
    
}