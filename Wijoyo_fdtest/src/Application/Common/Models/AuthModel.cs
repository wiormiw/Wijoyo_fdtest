namespace Wijoyo_fdtest.Application.Common.Models;

public record AuthResponseDto(
    string TokenType,
    string AccessToken,
    int ExpiresIn,
    string RefreshToken);

public record RegisterRequest(string Email, string Password, string UserName);
public record LoginRequest(string Email, string Password);
public record RefreshTokenRequest(string RefreshToken);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
public record ResetPasswordRequest(string Email);


