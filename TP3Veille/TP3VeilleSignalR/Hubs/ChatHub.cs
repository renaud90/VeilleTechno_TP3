using Microsoft.AspNetCore.SignalR;
using TP3VeilleSignalR.Data.Models;
using TP3VeilleSignalR.Services;
using TP3VeilleSignalR.Utilities;

namespace TP3VeilleSignalR.Hubs;

public record ChatMessage(string UserId, string ConversationId, string Message, DateTime Moment);
public record ChatUser(string UserId, IEnumerable<string> Friends, DateTime? LastTimeConnected);
public record ConversationInfo(string UserId, string ConversationId);
public record ConnectionResult(bool IsSuccess);

public class ChatHub : Hub
{
    private readonly IUsersService _usersService;

    public ChatHub(IUsersService usersService)
    {
        _usersService = usersService;
    }
    
    public async Task<ConnectionResult> Connect(string userId)
    {
        //var user = await _usersService.GetUserByName(userId) ?? await _usersService.CreateUser(userId);
        //if (user is null) 
            //return new ConnectionResult(false);
        
        //user.LastTimeConnected = DateTime.Now;
        //await _usersService.UpdateUser(user.Username, user);
        
        return new ConnectionResult(true);
    }

    public async Task<IEnumerable<ChatUser>> GetAllUsers()
    {
        //return (await _usersService.GetAllUsers()).ToChatUsers();
        return new[] { new ChatUser(UserId: "bobby123", Friends: Array.Empty<string>(), LastTimeConnected: DateTime.Now) };
    }

    public async Task JoinConversation(ConversationInfo info)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, info.ConversationId);
        await Clients.Group(info.ConversationId).SendAsync("UserJoined", new { info.UserId });
    }

    public async Task LeaveConversation(ConversationInfo info)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, info.ConversationId);
        await Clients.Group(info.ConversationId).SendAsync("UserLeft", new { info.UserId });
    }

    public async Task SendMessage(ChatMessage message)
    {
        await Clients.Group(message.ConversationId).SendAsync("ReceiveMessage", message);
    }

    public async Task<ConnectionResult> Disconnect(string userId)
    {
        return new ConnectionResult(true);
    }
}

