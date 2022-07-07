using Microsoft.AspNetCore.SignalR;
using TP3VeilleSignalR.Data.Models;
using TP3VeilleSignalR.Services;
using TP3VeilleSignalR.Utilities;

namespace TP3VeilleSignalR.Hubs;

public record ChatMessage(string UserId, string ConversationId, string Content, DateTime Moment);
public record ChatUser(string UserId, IEnumerable<string> Friends, DateTime? LastTimeConnected);
public record ConversationInfo(string UserId, string ConversationId);
public record ConnectionResult(bool IsSuccess);

public static class ChatHubEvents
{
    public const int ConnectionFailed = 101;
    public const int ConnectionSucceeded = 102;
}

public class ChatHub : Hub
{
    private readonly IUsersService _usersService;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(IUsersService usersService, ILogger<ChatHub> logger)
    {
        _usersService = usersService;
        _logger = logger;
    }
    
    public async Task<ConnectionResult> Connect(string userId)
    {
        var user = await _usersService.GetByUsernameAsync(userId) ?? await _usersService.CreateAsync(userId);

        if (user is null || user.ConnectionIds.Count > 0)
        {
            _logger.LogInformation(
                ChatHubEvents.ConnectionFailed,
                "[Connect] Username : {Username} cannot be logged in.",
                userId
            );
            return new ConnectionResult(false);
        }
        
        user.LastTimeConnected = DateTime.Now;
        user.ConnectionIds.Add(Context.ConnectionId);
        await _usersService.UpdateAsync(user.Username, user);
        
        _logger.LogInformation(
            ChatHubEvents.ConnectionSucceeded,
            "[Connect] : Username : {Username} connected successfully under connection ID {ConnectionId}",
            userId, Context.ConnectionId
            );
        return new ConnectionResult(true);
    }

    public async Task<IEnumerable<ChatUser>> GetAllUsers()
    {
        _logger.LogInformation("[GetAllUsers] Getting all users from service.");
        return (await _usersService.GetAllAsync(u => u.ConnectionIds.Count > 0)).ToChatUsers();
    }

    public async Task JoinConversation(ConversationInfo info)
    {
        if (!ValidateUser(info.UserId))
            return;
        
        await Groups.AddToGroupAsync(Context.ConnectionId, info.ConversationId);
        await Clients.Group(info.ConversationId).SendAsync("UserJoined", new { info.UserId });
    }

    public async Task LeaveConversation(ConversationInfo info)
    {
        if (!ValidateUser(info.UserId))
            return;

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, info.ConversationId);
        await Clients.Group(info.ConversationId).SendAsync("UserLeft", new { info.UserId });
    }

    public async Task SendMessage(ChatMessage message)
    {
        if (!ValidateUser(message.UserId))
            return;
        
        _logger.LogInformation(
            "[SendMessage : {Date:HH:mm:ss}] Sending message from {User} to conversation with id {ConversationId}",
            message.Moment,
            message.UserId,
            message.ConversationId
        );
        await Clients.GroupExcept(message.ConversationId, Context.ConnectionId).SendAsync("ReceiveMessage", message);
    }

    public async Task<ConnectionResult> Disconnect(string userId)
    {
        var user = await _usersService.GetByUsernameAsync(userId);
        if (!ValidateUser(user))
        {
            _logger.LogError(
                ChatHubEvents.ConnectionFailed,
                "[Disconnect] User with username {Username} was not found. Nothing to disconnect...",
                userId
            );
            return new ConnectionResult(false);
        }
        
        user!.ConnectionIds.Remove(Context.ConnectionId);
        await _usersService.UpdateAsync(user.Username, user);
        
        _logger.LogInformation(
            ChatHubEvents.ConnectionSucceeded,
            "[Disconnect] User {Username} disconnected successfully.",
            userId
        );
        return new ConnectionResult(true);
    }

    private bool ValidateUser(string userId)
    {
        var user = _usersService.GetByUsernameAsync(userId);
        user.Wait();
        var userIsValid = user.Result is not null && user.Result.ConnectionIds.Contains(Context.ConnectionId);
        _logger.LogInformation("Checking if user is valid : {Validity}", (userIsValid ? "Yes" : "No"));
        return userIsValid;
    }
    
    private bool ValidateUser(User? user)
    {
        var userIsValid = user is not null && user.ConnectionIds.Contains(Context.ConnectionId);
        _logger.LogInformation("Checking if user is valid : {Validity}", (userIsValid ? "Yes" : "No"));
        return userIsValid;
    }
}

