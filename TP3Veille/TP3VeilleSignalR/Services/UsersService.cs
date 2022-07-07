using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TP3VeilleSignalR.Data;
using TP3VeilleSignalR.Data.Models;

namespace TP3VeilleSignalR.Services;

public class UsersService : IUsersService
{
    private readonly IMongoCollection<User> _collection;
    private readonly ILogger<UsersService> _logger;
    
    public UsersService(IOptions<ChatDbSettings> settings, ILogger<UsersService> logger)
    {
        _logger = logger;
        var mongoClient = new MongoClient(settings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _collection = mongoDatabase.GetCollection<User>(settings.Value.UsersCollectionName);
    }
    
    public async Task<User?> GetByUsernameAsync(string username) =>
        await _collection.Find(u => u.Username == username).FirstOrDefaultAsync();
    
    public async Task<IEnumerable<User>> GetAllAsync()
        => await _collection.Find(_ => true).ToListAsync();

    public async Task<IEnumerable<User>> GetAllAsync(Expression<Func<User, bool>> predicate)
        => await _collection.Find(predicate).ToListAsync();
    

    public async Task<IEnumerable<User>> GetFriendsOfUserAsync(string username)
    {
        var user = await _collection.Find(u => u.Username == username).FirstOrDefaultAsync();
        return user is not null ? user.Friends : new List<User>();
    }

    public async Task<User?> CreateAsync(string username)
    {
        var userExists = await _collection.Find(u => u.Username == username).FirstOrDefaultAsync() is not null;

        if (userExists)
        {
            _logger.LogError("[CreateUser] User {User} already exists.", username);
            return null;
        }

        var user = new User
        {
            Username = username,
            TimeCreated = DateTime.Now
        };

        await _collection.InsertOneAsync(user);
        _logger.LogInformation("[CreateUser] User {User} was created at {Time:HH:mm:ss}.", user.Username, user.TimeCreated);
        return await _collection.Find(u => u.Username == username).FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(string username, User updatedUser)
        => await _collection.ReplaceOneAsync(u => u.Username == username, updatedUser);
}