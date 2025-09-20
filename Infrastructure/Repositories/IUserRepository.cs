using Ecotrack_Api.Domain;


namespace Ecotrack_Api.Infrastructure.Repositories;


public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(string id);
    Task CreateAsync(User user);
    Task UpdateAsync(User user);
    Task EnsureIndexesAsync();
}