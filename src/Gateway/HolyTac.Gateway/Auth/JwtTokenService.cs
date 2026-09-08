using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HolyTac.Gateway.Auth;

public class JwtTokenService(IOptions<AuthOptions> options)
{
    private readonly JwtOptions _jwt = options.Value.Jwt;

    public string IssueToken(StaffAccount account)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // El claim de rol se llama "role" a propósito (sin mapear a la URI larga de ClaimTypes.Role):
        // así RouteClaimsRequirement en ocelot.json puede compararlo tal cual ("role": "Gerente").
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, account.Username),
            new Claim("role", account.Role),
        };

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
