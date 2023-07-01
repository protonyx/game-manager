using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameManager.Application.Contracts;
using GameManager.Application.Services;
using Microsoft.IdentityModel.Tokens;

namespace GameManager.Server.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    private readonly SymmetricSecurityKey _signingKey;

    private readonly SigningCredentials _signingCredentials;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        
        var key = configuration["Jwt:Key"];
        byte[] keyBytes;

        if (string.IsNullOrWhiteSpace(key))
        {
            // Generate a random key
            keyBytes = new byte[16];
            Random.Shared.NextBytes(keyBytes);
        }
        else
        {
            keyBytes = Convert.FromBase64String(key);
        }
        
        _signingKey = new SymmetricSecurityKey(keyBytes);
        _signingCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
    }

    public SecurityKey GetSigningKey()
    {
        return _signingKey;
    }

    public string GenerateToken(ClaimsIdentity identity)
    {
        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            identity.Claims,
            expires: DateTime.Now.AddDays(30),
            signingCredentials: _signingCredentials);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}