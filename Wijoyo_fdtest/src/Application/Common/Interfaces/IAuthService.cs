using FluentEmail.Core;
using Wijoyo_fdtest.Application.Common.Models;

namespace Wijoyo_fdtest.Application.Common.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequest request, IFluentEmail fluentEmail);
    Task<AuthResponseDto> LoginAsync(LoginRequest request);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task ChangePasswordAsync(ChangePasswordRequest request);
    Task VerifyEmailAsync(string userId, string token);
    Task SendPasswordResetLinkAsync(string email, IFluentEmail fluentEmail);
    Task<string> OneClickPasswordResetAsync(string token);
}

