using Microsoft.AspNetCore.SignalR;

namespace TP3VeilleSignalR.Hubs;

public record ChatMessage(string UserId, string ConversationId, string Message, DateTime Timestamp);
public record ChatJoinOrLeave(string UserId, string ConversationId);

public class ChatHub : Hub
{
    public async Task Join(ChatJoinOrLeave payload)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, payload.ConversationId);
        await Clients.Group(payload.ConversationId).SendAsync("UserJoined", new { payload.UserId });
    }

    public async Task Leave(ChatJoinOrLeave payload)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, payload.ConversationId);
        await Clients.Group(payload.ConversationId).SendAsync("UserLeft", new { payload.UserId });
    }
    
    public async Task SendMessage(ChatMessage message)
    {
        await Clients.Group(message.ConversationId).SendAsync("ReceiveMessage", message);
    }
}

