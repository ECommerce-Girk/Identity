using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Identity.Core.Authentication;
using Identity.Core.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Infra.Authentication;

public sealed class TokenProvider : ITokenProvider
{
    private readonly string issuer;
    private readonly string audience;
    private readonly SigningCredentials credentials;

    public TokenProvider(IConfiguration config)
    {
        var jwtSettings = config.GetSection("JwtSettings");
        issuer = jwtSettings["Issuer"]!;
        audience = jwtSettings["Audience"]!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
        credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    public string Create(User user)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Email, user.Email)
            ]),
            //Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ExpirationInMinutes")),
            SigningCredentials = credentials,
            Issuer = issuer,
            Audience = audience
        };

        var handler = new JsonWebTokenHandler();

        return handler.CreateToken(tokenDescriptor);
    }

    public string GenerateRefreshToken()
    {
        byte[] randomBytes = RandomNumberGenerator.GetBytes(32);

        return Convert.ToBase64String(randomBytes);
    }
}
