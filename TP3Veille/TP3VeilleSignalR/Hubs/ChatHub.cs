using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.SignalR;
using TP3VeilleSignalR.Data.Models;
using TP3VeilleSignalR.Services;
using TP3VeilleSignalR.Utilities;

namespace TP3VeilleSignalR.Hubs;
public record ChatUser(string UserId, DateTime? LastTimeConnected);
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
    private readonly IConversationsService _conversationsService;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(IUsersService usersService, ILogger<ChatHub> logger, IConversationsService conversationsService)
    {
        _usersService = usersService;
        _conversationsService = conversationsService;
        _logger = logger;
    }
    
    public async Task<Result<UserData>> Connect(string userId)
    {
        var users = await _usersService.GetAllAsync(u => u.ConnectionIds.Contains(Context.ConnectionId));
        if (users.Any())
        {
            _logger.LogWarning(
                ChatHubEvents.ConnectionFailed,
                "[{Method}] Connection already in use by another user.",
                nameof(Connect)
            );
            return Result.Fail<UserData>($"Connection already used by another user.");
        }

        var user = await _usersService.GetByUsernameAsync(userId) ?? await _usersService.CreateAsync(userId);
        if (user is null || user.ConnectionIds.Count > 0)
        {
            _logger.LogWarning(
                ChatHubEvents.ConnectionFailed,
                "[{Method}] Username : {Username} cannot be logged in.",
                nameof(Connect),
                userId
            );
            return Result.Fail<UserData>($"Cannot connect user {userId}.");
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
        
        var userData = await _conversationsService.GetConversationsDataForUser(user.Username);

        await Clients.All.SendAsync("UserConnected");
        return Result.Ok(userData);
    }

    public async Task<IEnumerable<ChatUser>> GetAllUsers()
    {
        _logger.LogInformation("[{Method}] Getting all users from service.", nameof(GetAllUsers));
        return (await _usersService.GetAllAsync(u => u.ConnectionIds.Count > 0)).ToChatUsers();
    }

    public string GetConversationId(string userId, string otherUserId)
    {
        if (string.Compare(userId, otherUserId, StringComparison.Ordinal) <= 0)
            return $"{userId}:{otherUserId}";
        else
            return $"{otherUserId}:{userId}";
    }
    
    public async Task<Result<IEnumerable<Message>>> JoinConversation(ConversationInfo info)
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
            return Result.Fail<IEnumerable<Message>>($"Joining {info.ConversationId} has failed. Wrong user for the current connection.");
        }

        var conversation = await _conversationsService.ExistsAsync(info.ConversationId)
            ? await _conversationsService.GetByIdAsync(info.ConversationId)
            : await _conversationsService.CreateAsync(info.ConversationId);

        if (conversation is null)
        {
            _logger.LogWarning(
                ChatHubEvents.JoiningFailed,
                "[{Method}] Conversation with id {ConversationId} was not found or was invalid",
                nameof(JoinConversation),
                info.ConversationId
            );
            return Result.Fail<IEnumerable<Message>>($"Joining {info.ConversationId} has failed. Conversation invalid.");
        }

        var userAdded = await _conversationsService.AddUserToConversationAsync(info.ConversationId, info.UserId);
        
        if (!userAdded && !conversation.Participants.Contains(info.UserId))
        {
            _logger.LogWarning(
                ChatHubEvents.JoiningFailed,
                "[{Method}] User {User} cannot be added to conversation with id {ConversationId}",
                nameof(JoinConversation),
                info.UserId,
                info.ConversationId
            );
            return Result.Fail<IEnumerable<Message>>($"Joining {info.ConversationId} has failed. Problem occured during adding user to conversation.");
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
        _logger.LogInformation("Old messages count : {MessageCount}", conversation.Messages.Count);
        return Result.Ok<IEnumerable<Message>>(conversation.Messages);
    }

    public async Task<Result> LeaveConversation(ConversationInfo info)
    {
        if (!ValidateUser(info.UserId))
            return Result.Fail($"User {info.UserId} cannot leave conversation {info.ConversationId}.");
        
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, info.ConversationId);
        await Clients.Group(info.ConversationId).SendAsync("UserLeft", new { info.UserId });
        return Result.Ok();
    }

    public async Task<Result> SendMessage(Message message)
    {
        if (!ValidateUser(message.UserId))
        {
            _logger.LogWarning(
                "[{Method} ({Date:HH:mm:ss}] Message from {User} cannot be sent to conversation with id {ConversationId}.",
                nameof(SendMessage),
                message.Moment,
                message.UserId,
                message.ConversationId
            );
            return Result.Fail("Message cannot be sent.");
        }

        var messageAdded = await _conversationsService.AddMessageToConversationAsync(message.ConversationId, message);

        if (!messageAdded)
        {
            _logger.LogWarning(
                "[{Method}] ({Date:HH:mm:ss}] Message from {User} could not be added to conversation with id {ConversationId}",
                nameof(SendMessage),
                message.Moment,
                message.UserId,
                message.ConversationId
            );
            return Result.Fail("Message could not be added to database.");
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
        if (!ValidateUser(userId))
        {
            _logger.LogWarning(
                ChatHubEvents.ConnectionFailed,
                "[{Method}] User with username {Username} was not valid. Nothing to disconnect...",
                nameof(Disconnect),
                userId
            );
            return Result.Fail($"User {userId} cannot be disconnected.");
        }
        
        var user = await _usersService.GetByUsernameAsync(userId);
        
        if (user is null)
        {
            _logger.LogWarning(
                ChatHubEvents.ConnectionFailed,
                "[{Method}] User with username {Username} was not found. Nothing to disconnect...",
                nameof(Disconnect),
                userId
            );
            return Result.Fail($"User {userId} was not found.");
        }
        
        user.ConnectionIds.Remove(Context.ConnectionId);
        await _usersService.UpdateAsync(user.Username, user);
        
        _logger.LogInformation(
            ChatHubEvents.ConnectionSucceeded,
            "[{Method}] User {Username} disconnected successfully.",
            nameof(Disconnect),
            userId
        );
        await Clients.All.SendAsync("UserDisconnected");
        return Result.Ok();
    }
    
    private bool ValidateUser(string userId)
    {
        var users = _usersService.GetAllAsync(u => u.ConnectionIds.Contains(Context.ConnectionId));
        users.Wait();
        var user = users.Result.FirstOrDefault(u => u.Username == userId);
        var userIsValid = user is not null && users.Result.Count() == 1;
        _logger.LogInformation("Checking if user is valid : {Validity}", (userIsValid ? "Yes" : "No"));
        return userIsValid;
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception is not null)
        {
            _logger.LogError(
                "Exception occured that caused disconnection of {ConnectionId}.\n{ExceptionMessage}",
                Context.ConnectionId,
                exception.Message
            );
            
            var user = (await _usersService.GetAllAsync(u => u.ConnectionIds.Contains(Context.ConnectionId))).FirstOrDefault();
            if (user is null)
            {
                _logger.LogError(
                    "User not found while {ConnectionId}.\n{ExceptionMessage}",
                    Context.ConnectionId,
                    exception.Message
                );
                return;
            }

            await Disconnect(user.Username);
        }
    }
}

