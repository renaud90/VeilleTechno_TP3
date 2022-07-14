using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TP3VeilleSignalR.Data;
using TP3VeilleSignalR.Data.Models;

namespace TP3VeilleSignalR.Services;

public record ConversationData(string ConversationId, string InterlocutorId);

public record UserData(string UserId, IEnumerable<ConversationData> Conversations);

public class ConversationsService : IConversationsService
{
    private readonly IMongoCollection<Conversation> _conversationsCollection;
    private readonly IMongoCollection<User> _usersCollection;
    private readonly ILogger<ConversationsService> _logger;
    
    public ConversationsService(IOptions<ChatDbSettings> settings, ILogger<ConversationsService> logger)
    {
        _logger = logger;
        var mongoClient = new MongoClient(settings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _conversationsCollection = mongoDatabase.GetCollection<Conversation>(settings.Value.ConversationsCollectionName);
        _usersCollection = mongoDatabase.GetCollection<User>(settings.Value.UsersCollectionName);
    }
    
    public async Task<bool> ExistsAsync(string conversationId) 
        => await _conversationsCollection.Find(c => c.Name == conversationId).AnyAsync();

    public async Task<Conversation?> GetByIdAsync(string conversationId) 
        => await _conversationsCollection.Find(c => c.Name == conversationId).FirstOrDefaultAsync();

    public async Task<IEnumerable<Message>> GetMessagesForConversationAsync(string conversationId)
    {
        var conversation = await _conversationsCollection.Find(c => c.Name == conversationId).FirstOrDefaultAsync();
        return conversation is null ? new List<Message>() : conversation.Messages;
    }

    public async Task<IEnumerable<Message>> GetMessagesForConversationAsync(string conversationId, Func<Message, bool> predicate)
    {
        var conversation = await _conversationsCollection.Find(c => c.Name == conversationId).FirstOrDefaultAsync();
        return conversation is null ? new List<Message>() : conversation.Messages.Where(predicate);
    }

    public async Task<UserData> GetConversationsDataForUser(string userId)
    {
        var user = await _usersCollection.Find(u => u.Username == userId).FirstOrDefaultAsync();

        if (user is null)
            return new UserData(userId, new List<ConversationData>());

        var conversations = await _conversationsCollection.Find(c => c.Participants.Contains(userId)).ToListAsync();
        if (conversations is null || !conversations.Any())
        {
            return new UserData(userId, new List<ConversationData>());
        }

        return new UserData(
            userId, 
            conversations.Select(c =>
                new ConversationData(c.Name, c.Participants.FirstOrDefault(p => p != userId) ?? string.Empty)
            )
        );
    }
    
    public async Task<Conversation?> CreateAsync(string conversationId)
    {
        var conversation = await _conversationsCollection.Find(c => c.Name == conversationId).FirstOrDefaultAsync();

        if (conversation is not null)
            return null;

        conversation = new Conversation
        {
            Name = conversationId,
            TimeCreated = DateTime.Now
        };

        await _conversationsCollection.InsertOneAsync(conversation);
        return await GetByIdAsync(conversationId);
    }

    public async Task<bool> AddUserToConversationAsync(string conversationId, string username)
    {
        var conversation = await _conversationsCollection.Find(c => c.Name == conversationId).FirstOrDefaultAsync();

        if (!CanAddUserToConversation(username, conversation))
            return false;
        
        var update = Builders<Conversation>.Update.Push(c => c.Participants, username);
        var result = await _conversationsCollection.UpdateOneAsync(c => c.Name == conversationId, update);
        return (result.IsAcknowledged && result.IsModifiedCountAvailable && result.ModifiedCount > 0);
    }
    
    public async Task<bool> AddMessageToConversationAsync(string conversationId, Message message)
    {
        if (!ValidateMessage(message))
            return false;
        
        var update = Builders<Conversation>.Update
            .Set(c=> c.LastMessageTime, message.Moment)
            .Push(c => c.Messages, message);
        var result = await _conversationsCollection.UpdateOneAsync(c => c.Name == conversationId, update);
        return (result.IsAcknowledged && result.IsModifiedCountAvailable && result.ModifiedCount > 0);
    }
    
    private static bool CanAddUserToConversation(string username, Conversation? conversation)
    {
        return conversation is not null && !conversation.Participants.Contains(username)
                                        && conversation.Participants.Count < 2;
    }
    
    private static bool ValidateMessage(Message message)
    {
        return !string.IsNullOrEmpty(message.Content) && !string.IsNullOrEmpty(message.UserId) &&
               !string.IsNullOrEmpty(message.ConversationId);
    }
}