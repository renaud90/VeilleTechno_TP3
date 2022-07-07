using System.Linq.Expressions;
using TP3VeilleSignalR.Data.Models;

namespace TP3VeilleSignalR.Services;

public interface IUsersService
{
    public Task<User?> GetByUsernameAsync(string username);
    public Task<IEnumerable<User>> GetAllAsync();
    public Task<IEnumerable<User>> GetAllAsync(Expression<Func<User, bool>> predicate);
    public Task<IEnumerable<User>> GetFriendsOfUserAsync(string username);
    public Task<User?> CreateAsync(string username);
    public Task UpdateAsync(string username, User updatedUser);
}