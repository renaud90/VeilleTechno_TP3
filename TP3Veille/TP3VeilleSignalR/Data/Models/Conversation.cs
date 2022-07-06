using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TP3VeilleSignalR.Data.Models;

public class Conversation
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<string> Participants { get; set; }
    public IEnumerable<Message> Messages { get; set; }
    public DateTime TimeCreated { get; set; }
    public DateTime? LastMessageTime { get; set; }
}