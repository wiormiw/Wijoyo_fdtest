using System.Security.Claims;

namespace Wijoyo_fdtest.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(string userId, string email);
    string GenerateRefreshToken(string userId, string email);
    int GetAccessTokenExpiryInSeconds();
    int GetRefreshTokenExpiryInSeconds();
    ClaimsPrincipal? ValidateRefreshToken(string token);
}

