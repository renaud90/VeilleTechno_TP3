using Microsoft.AspNetCore.SignalR;
using TP3VeilleSignalR.Data.Models;
using TP3VeilleSignalR.Services;
using TP3VeilleSignalR.Utilities;

namespace TP3VeilleSignalR.Hubs;

public record ChatMessage(string UserId, string ConversationId, string Message, DateTime Moment);
public record ChatUser(string UserId, IEnumerable<string> Friends, DateTime? LastTimeConnected);
public record ConversationInfo(string UserId, string ConversationId);

public static class ChatHubEvents
{
    public const int ConnectionFailed = 101;
    public const int ConnectionSucceeded = 102;
    public const int JoiningFailed = 103;
    public const int JoiningSucceeded = 104;
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
    
    public async Task<Result<ChatUser>> Connect(string userId)
    {
        var user = await _usersService.GetByUsernameAsync(userId) ?? await _usersService.CreateAsync(userId);

        if (user is null || user.ConnectionIds.Count > 0)
        {
            _logger.LogWarning(
                ChatHubEvents.ConnectionFailed,
                "[{Method}] Username : {Username} cannot be logged in.",
                nameof(Connect),
                userId
            );
            return Result.Fail<ChatUser>($"Cannot connect user {userId}.");
        }
        
        user.LastTimeConnected = DateTime.Now;
        user.ConnectionIds.Add(Context.ConnectionId);
        await _usersService.UpdateAsync(user.Username, user);

        _logger.LogInformation(
            ChatHubEvents.ConnectionSucceeded,
            "[{Method}] : Username : {Username} connected successfully under connection ID {ConnectionId}",
            nameof(Connect),
            userId,
            Context.ConnectionId
        );
        return Result.Ok<ChatUser>(user.ToChatUser());
    }

    public async Task<IEnumerable<ChatUser>> GetAllUsers()
    {
        _logger.LogInformation("[{Method}] Getting all users from service.", nameof(GetAllUsers));
        return (await _usersService.GetAllAsync(u => u.ConnectionIds.Count > 0)).ToChatUsers();
    }

    public async Task<Result<IEnumerable<ChatMessage>>> JoinConversation(ConversationInfo info)
    {

        if (!ValidateUser(info.UserId))
        {
            _logger.LogWarning(
                ChatHubEvents.JoiningFailed,
                "[{Method}] User {User} could not join conversation with id {ConversationId}",
                nameof(JoinConversation),
                info.UserId,
                info.ConversationId
            );
            return Result.Fail<IEnumerable<ChatMessage>>($"Joining {info.ConversationId} has failed.");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, info.ConversationId);
        await Clients.Group(info.ConversationId).SendAsync("UserJoined", new { info.UserId });
        _logger.LogInformation(
            ChatHubEvents.JoiningSucceeded,
            "[{Method}] User {User} successfully joined conversation with id {ConversationId}",
            nameof(JoinConversation),
            info.UserId,
            info.ConversationId
        );
        return Result.Ok<IEnumerable<ChatMessage>>(new List<ChatMessage>());
    }

    public async Task<Result> LeaveConversation(ConversationInfo info)
    {
        if (!ValidateUser(info.UserId))
            return Result.Fail($"User {info.UserId} cannot leave conversation {info.ConversationId}.");

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, info.ConversationId);
        await Clients.Group(info.ConversationId).SendAsync("UserLeft", new { info.UserId });
        return Result.Ok();
    }

    public async Task<Result> SendMessage(ChatMessage message)
    {

        if (!ValidateUser(message.UserId))
        {
            _logger.LogWarning(
                "[{Method} ({Date;HH:mm:ss}] Message from {User} cannot be sent to conversation with id {ConversationId}.",
                nameof(SendMessage),
                message.Moment,
                message.UserId,
                message.ConversationId
            );
            return Result.Fail("Message cannot be sent.");
        }

        _logger.LogInformation(
            "[{Method} ({Date:HH:mm:ss})] Sending message from {User} to conversation with id {ConversationId}",
            nameof(SendMessage),
            message.Moment,
            message.UserId,
            message.ConversationId
        );

        await Clients.GroupExcept(message.ConversationId, Context.ConnectionId).SendAsync("ReceiveMessage", message);
        return Result.Ok();
    }

    public async Task<Result> Disconnect(string userId)
    {
        var user = await _usersService.GetByUsernameAsync(userId);
        if (!ValidateUser(user))
        {
            _logger.LogWarning(
                ChatHubEvents.ConnectionFailed,
                "[Disconnect] User with username {Username} was not found. Nothing to disconnect...",
                userId
            );
            return Result.Fail($"User {userId} cannot be disconnected.");
        }
        
        user!.ConnectionIds.Remove(Context.ConnectionId);
        await _usersService.UpdateAsync(user.Username, user);
        
        _logger.LogInformation(
            ChatHubEvents.ConnectionSucceeded,
            "[Disconnect] User {Username} disconnected successfully.",
            userId
        );
        return Result.Ok();
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

