using Microsoft.AspNetCore.SignalR;

namespace TP3VeilleSignalR.Hubs;

public record ChatMessage(string UserId, string ConversationId, string Message, DateTime Timestamp);
public record GroupJoinOrLeave(string UserId, string GroupId);

public record ConnectionResult(bool IsSuccess);

public class ChatHub : Hub
{
    public async Task<ConnectionResult> Connect(string user)
    {
        return new ConnectionResult(true);
    }

    public async Task<IEnumerable<object>> GetAllUsers()
    {
       return new[] { new { userId = "Bobby", isConnected = true } };
    }

    public async Task JoinGroup(GroupJoinOrLeave payload)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, payload.GroupId);
        await Clients.Group(payload.GroupId).SendAsync("UserJoined", new { payload.UserId });
    }

    public async Task LeaveGroup(GroupJoinOrLeave payload)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, payload.GroupId);
        await Clients.Group(payload.GroupId).SendAsync("UserLeft", new { payload.UserId });
    }

    public async Task SendMessage(ChatMessage message)
    {
        await Clients.Group(message.ConversationId).SendAsync("ReceiveMessage", message);
    }

    public async Task Disconnect(string userId)
    {

    }
}

