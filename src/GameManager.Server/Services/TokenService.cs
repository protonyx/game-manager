﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace GameManager.Server.Services;

public class TokenService
{
    private readonly IConfiguration _configuration;

    private readonly SigningCredentials _signingCredentials;

    public static byte[] DefaultKey;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        
        var key = configuration["Jwt:Key"];
        var keyBytes = string.IsNullOrWhiteSpace(key)
            ? DefaultKey
            : Convert.FromBase64String(key);
        
        var securityKey = new SymmetricSecurityKey(keyBytes);
        _signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    }

    public string GenerateToken(Guid gameId, Guid playerId, bool isAdmin)
    {
        var claims = new List<Claim>()
        {
            new Claim("name", playerId.ToString()),
            new Claim("sid", gameId.ToString())
        };

        if (isAdmin)
        {
            claims.Add(new Claim("role", "admin"));
        }
        
        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddDays(30),
            signingCredentials: _signingCredentials);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}