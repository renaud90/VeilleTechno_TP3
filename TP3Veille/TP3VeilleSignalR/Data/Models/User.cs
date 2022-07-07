using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TP3VeilleSignalR.Data.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Username { get; set; }
    public IList<User> Friends { get; set; } = new List<User>();
    public IList<string> ConnectionIds { get; set; } = new List<string>();
    public DateTime TimeCreated { get; set; }
    public DateTime? LastTimeConnected { get; set; }
}