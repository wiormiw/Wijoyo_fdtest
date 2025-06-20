using System.Security.Claims;

namespace Wijoyo_fdtest.Application.Common.Interfaces;

public interface IVerificationTokenService
{
    string GenerateEmailVerificationToken(string userId, string email);
    ClaimsPrincipal? ValidateEmailVerificationToken(string token);
    string GenerateOneClickPasswordResetToken(string userId, string email);
    ClaimsPrincipal? ValidateOneClickPasswordResetToken(string token);

}
