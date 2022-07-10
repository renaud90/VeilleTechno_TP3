using System.Linq.Expressions;
using TP3VeilleSignalR.Data.Models;
using TP3VeilleSignalR.Hubs;

namespace TP3VeilleSignalR.Services;

public interface IConversationsService
{
    public Task<bool> ExistsAsync(string conversationId);
    public Task<Conversation> GetByIdAsync(string conversationId);
    public Task<IEnumerable<Message>> GetMessagesForConversationAsync(string conversationId);
    public Task<IEnumerable<Message>> GetMessagesForConversationAsync(string conversationId, Expression<Func<Message, bool>> predicate);
    public Task<IEnumerable<User>> GetUsersForConversationAsync(string conversationId);
    public Task<bool> AddUserToConversationAsync(string conversationId, string username);
    public Task<bool> RemoveUserFromConversationAsync(string conversationId, string username);
    public Task<bool> AddMessageToConversationAsync(string conversationId, ChatMessage message);
    
    
}