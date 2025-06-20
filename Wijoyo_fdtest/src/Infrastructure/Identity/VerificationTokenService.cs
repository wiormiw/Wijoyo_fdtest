using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Wijoyo_fdtest.Application.Common.Interfaces;

namespace Wijoyo_fdtest.Infrastructure.Identity;

public class VerificationTokenService : IVerificationTokenService
{
    private readonly IConfiguration _config;
    private readonly string _secretKey;

    public VerificationTokenService(IConfiguration config)
    {
        _config = config;
        _secretKey = _config["Jwt:EmailVerificationSecret"] 
                     ?? throw new Exception("Missing EmailVerificationSecret.");
    }

    public string GenerateEmailVerificationToken(string userId, string email)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secretKey);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: "wijoyo_fdtest",
            audience: "wijoyo_fdtest",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );

        return tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal? ValidateEmailVerificationToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secretKey);

        try
        {
            return tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = "wijoyo_fdtest",
                ValidAudience = "wijoyo_fdtest",
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.FromMinutes(5)
            }, out _);
        }
        catch
        {
            return null;
        }
    }
    
    public string GenerateOneClickPasswordResetToken(string userId, string email)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secretKey);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim("purpose", "one_click_reset"),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: "wijoyo_fdtest",
            audience: "wijoyo_fdtest",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );

        return tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal? ValidateOneClickPasswordResetToken(string token)
    {
        var principal = ValidateEmailVerificationToken(token);
        var purpose = principal?.FindFirst("purpose")?.Value;
        return purpose == "one_click_reset" ? principal : null;
    }
}
