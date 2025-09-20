using Ecotrack_Api.Domain;
using MongoDB.Driver;


namespace Ecotrack_Api.Infrastructure.Repositories;


public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _col;


    public UserRepository(IMongoContext ctx)
    {
        _col = ctx.Users;
    }


    public async Task<User?> GetByEmailAsync(string email)
    => await _col.Find(u => u.Email == email).FirstOrDefaultAsync();


    public async Task<User?> GetByIdAsync(string id)
    => await _col.Find(u => u.Id == id).FirstOrDefaultAsync();


    public async Task CreateAsync(User user)
    => await _col.InsertOneAsync(user);


    public async Task UpdateAsync(User user)
    => await _col.ReplaceOneAsync(u => u.Id == user.Id, user);


    public async Task EnsureIndexesAsync()
    {
        var emailIdx = new CreateIndexModel<User>(
        Builders<User>.IndexKeys.Ascending(u => u.Email),
        new CreateIndexOptions { Unique = true, Name = "ux_email" }
        );
        await _col.Indexes.CreateOneAsync(emailIdx);


        var orgIdx = new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(u => u.OrganizationId), new CreateIndexOptions { Name = "ix_org" });
        await _col.Indexes.CreateOneAsync(orgIdx);
    }
}
