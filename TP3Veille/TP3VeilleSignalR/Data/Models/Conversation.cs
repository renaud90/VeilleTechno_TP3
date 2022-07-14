using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TP3VeilleSignalR.Data.Models;

public class Message
{
    public string Content { get; set; }
    public string UserId { get; set; }
    public string ConversationId { get; set; }
    public DateTime Moment { get; set; }
}

public class Conversation
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Name { get; set; }
    public IList<string> Participants { get; set; } = new List<string>();
    public IList<Message> Messages { get; set; } = new List<Message>();
    public DateTime TimeCreated { get; set; }
    public DateTime? LastMessageTime { get; set; }
}