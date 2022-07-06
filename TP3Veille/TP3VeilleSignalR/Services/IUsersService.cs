using TP3VeilleSignalR.Data.Models;

namespace TP3VeilleSignalR.Services;

public interface IUsersService
{
    public Task<User?> GetUserByName(string username);
    public Task<IEnumerable<User>> GetUsersFromConversation(string conversationId);
    public Task<IEnumerable<User>> GetAllUsers();
    public Task<IEnumerable<User>> GetUserFriends(string username);
    public Task<User?> CreateUser(string username);
    public Task UpdateUser(string username, User updatedUser);
}