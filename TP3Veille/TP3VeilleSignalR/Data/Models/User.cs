using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TP3VeilleSignalR.Data.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Username { get; set; }
    public IEnumerable<User> Friends { get; set; }
    public IEnumerable<string> ConnectionIds { get; set; }
    public DateTime TimeCreated { get; set; }
    public DateTime? LastTimeConnected { get; set; }
}