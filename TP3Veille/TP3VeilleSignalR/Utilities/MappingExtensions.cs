using TP3VeilleSignalR.Data.Models;
using TP3VeilleSignalR.Hubs;

namespace TP3VeilleSignalR.Utilities;

public static class MappingExtensions
{
    public static ChatUser ToChatUser(this User user)
    {
        return new ChatUser(
            UserId: user.Username,
            Friends: user.Friends.Select(f => f.Username),
            LastTimeConnected: user.LastTimeConnected
        );
    }

    public static IEnumerable<ChatUser> ToChatUsers(this IEnumerable<User> users)
    {
        return users.Select(u => u.ToChatUser());
    }
}