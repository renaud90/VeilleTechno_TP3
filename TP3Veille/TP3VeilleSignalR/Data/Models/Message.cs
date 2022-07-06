using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TP3VeilleSignalR.Data.Models;

public class Message
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Content { get; set; }
    public string SenderId { get; set; }
    public string ConversationId { get; set; }
    public DateTime Moment { get; set; }
}