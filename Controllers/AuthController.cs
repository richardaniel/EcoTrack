using Ecotrack_Api.Application.Auth;
using Microsoft.AspNetCore.Mvc;


namespace Ecotrack_Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;


    public record RegisterRequest(string OrganizationId, string Name, string Email, string Password, string Role = "viewer");
    public record LoginRequest(string Email, string Password);


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var (ok, msg) = await _auth.RegisterAsync(req.OrganizationId, req.Name, req.Email, req.Password, req.Role);
        return ok ? Ok(new { message = msg }) : BadRequest(new { message = msg });
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var (ok, token, msg) = await _auth.LoginAsync(req.Email, req.Password);
        return ok ? Ok(new { token }) : Unauthorized(new { message = msg });
    }
}
