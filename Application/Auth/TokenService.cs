using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ecotrack_Api.Config;
using Ecotrack_Api.Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace Ecotrack_Api.Application.Auth;


public interface ITokenService
{
    string CreateToken(User user);
}


public class TokenService : ITokenService
{
    private readonly JwtOptions _opt;
    public TokenService(IOptions<JwtOptions> opt) => _opt = opt.Value;


    public string CreateToken(User user)
    {
        var claims = new List<Claim>
{
new(JwtRegisteredClaimNames.Sub, user.Id),
new(JwtRegisteredClaimNames.Email, user.Email),
new(ClaimTypes.Role, user.Role),
new("org", user.OrganizationId)
};


        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


        var token = new JwtSecurityToken(
        issuer: _opt.Issuer,
        audience: _opt.Audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(_opt.ExpiresMinutes),
        signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}