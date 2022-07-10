namespace TP3VeilleSignalR.Data.Models;

public class Message
{
    public string Content { get; set; }
    public string UserId { get; set; }
    public string ConversationId { get; set; }
    public DateTime Moment { get; set; }
}