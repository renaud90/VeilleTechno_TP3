using Microsoft.AspNetCore.SignalR;

namespace TP3VeilleSignalR.Hubs;

public record ChatMessage(string UserId, string ConversationId, string Message, DateTime Timestamp);
public record GroupJoinOrLeave(string UserId, string GroupId);

public class ChatHub : Hub
{
    public async Task Connect(string user)
    {
        await Clients.Caller.SendAsync("ConnectionResult", new { IsSuccess = true });
    }

    public async Task GetAllUsers()
    {
        await Clients.Caller.SendAsync("UserData", new[] { new { userId = "Bobby", isConnected = true } });
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

