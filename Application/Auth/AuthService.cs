using Ecotrack_Api.Domain;
using Ecotrack_Api.Infrastructure.Repositories;


namespace Ecotrack_Api.Application.Auth;


public interface IAuthService
{
    Task<(bool ok, string message)> RegisterAsync(string orgId, string name, string email, string password, string role = "viewer");
    Task<(bool ok, string? token, string message)> LoginAsync(string email, string password);
}


public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly ITokenService _tokens;


    public AuthService(IUserRepository users, ITokenService tokens)
    { _users = users; _tokens = tokens; }


    public async Task<(bool ok, string message)> RegisterAsync(string orgId, string name, string email, string password, string role = "viewer")
    {
        var existing = await _users.GetByEmailAsync(email.ToLowerInvariant());
        if (existing != null) return (false, "El correo ya existe");


        var user = new User
        {
            OrganizationId = orgId,
            Name = name,
            Email = email.ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = role
        };
        await _users.CreateAsync(user);
        return (true, "Usuario creado");
    }


    public async Task<(bool ok, string? token, string message)> LoginAsync(string email, string password)
    {
        var user = await _users.GetByEmailAsync(email.ToLowerInvariant());
        if (user == null) return (false, null, "Credenciales inválidas");
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return (false, null, "Credenciales inválidas");


        user.LastLoginAt = DateTime.UtcNow;
        await _users.UpdateAsync(user);


        var token = _tokens.CreateToken(user);
        return (true, token, "OK");
    }
}