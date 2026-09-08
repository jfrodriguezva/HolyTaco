using HolyTac.Gateway.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace HolyTac.Gateway.Controllers;

public record LoginRequest(string Username, string Password);
public record LoginResponse(string Token, string Username, string Role, DateTime ExpiresAtUtc);

[ApiController]
[Route("api/auth")]
[EnableRateLimiting("login")]
public class AuthController(IOptions<AuthOptions> options, JwtTokenService tokenService) : ControllerBase
{
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login(LoginRequest request)
    {
        var account = options.Value.Accounts
            .FirstOrDefault(a => string.Equals(a.Username, request.Username, StringComparison.OrdinalIgnoreCase));

        if (account is null || !PasswordHasher.Verify(request.Password, account.PasswordHash))
            return Unauthorized(new { message = "Usuario o contraseña incorrectos." });

        var token = tokenService.IssueToken(account);
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(options.Value.Jwt.ExpiryMinutes);

        return Ok(new LoginResponse(token, account.Username, account.Role, expiresAtUtc));
    }
}
