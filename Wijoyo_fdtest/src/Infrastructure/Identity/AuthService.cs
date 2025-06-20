using System.Security.Claims;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Wijoyo_fdtest.Application.Common.Interfaces;
using Wijoyo_fdtest.Application.Common.Models;
using Wijoyo_fdtest.Domain.Constants;

namespace Wijoyo_fdtest.Infrastructure.Identity;

public class AuthService : IAuthService
{
    private readonly IUser _user;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IVerificationTokenService _verificationTokenService;

    public AuthService(
        IUser user,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenGenerator jwtTokenGenerator,
        IVerificationTokenService verificationTokenService)
    {
        _user = user;
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _verificationTokenService = verificationTokenService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequest request, IFluentEmail fluentEmail)
    {
        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            throw new ApplicationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        await _userManager.AddToRoleAsync(user, Roles.User);

        // Email Verification Request (Not yet verified).
        var token = _verificationTokenService.GenerateEmailVerificationToken(user.Id, user.Email!);
        
        var verifyUrl = $"https://localhost:5001/api/Auth/verify-email?token={token}";
        
        await fluentEmail
            .To(user.Email)
            .Subject("Email verification for Wijoyo_fdtest")
            .Body($"Click here to verify your email: <a href='{verifyUrl}'>Verify</a>", isHtml: true)
            .SendAsync();

        return await GenerateAuthResultAsync(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }   

        return await GenerateAuthResultAsync(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var principal = _jwtTokenGenerator.ValidateRefreshToken(refreshToken);
        if (principal == null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = principal.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(email))
        {
            throw new UnauthorizedAccessException("Invalid token claims.");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException(nameof(user), "User not found.");
        }

        return await GenerateAuthResultAsync(user);
    }

    public async Task ChangePasswordAsync(ChangePasswordRequest request)
    {
        var userId = _user.Id;
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("Can't handle the request because you unauthorized.");
        }
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            throw new NotFoundException(userId, "User not found.");
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            throw new ApplicationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    public async Task VerifyEmailAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            throw new ApplicationException("Email verification failed.");
        }
    }

    public async Task SendPasswordResetLinkAsync(string email, IFluentEmail fluentEmail)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return;
        }

        var token = _verificationTokenService.GenerateOneClickPasswordResetToken(user.Id, user.Email!);
        var url = $"https://localhost:5001/api/Auth/one-click-reset?token={token}";

        await fluentEmail
            .To(email)
            .Subject("Reset Your Password - Wijoyo_fdtest")
            .Body($"Click here to reset your password: <a href='{url}'>Reset Password</a>", isHtml: true)
            .SendAsync();
    }
    
    public async Task<string> OneClickPasswordResetAsync(string token)
    {
        var principal = _verificationTokenService.ValidateOneClickPasswordResetToken(token);
        if (principal == null)
        {
            throw new UnauthorizedAccessException("Invalid or expired token.");
        }

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var user = await _userManager.FindByIdAsync(userId!);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        var newPassword = GenerateRandomPassword();
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

        if (!result.Succeeded)
        {
            throw new ApplicationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return newPassword;
    }

    private Task<AuthResponseDto> GenerateAuthResultAsync(ApplicationUser user)
    {
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user.Id, user.Email!);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken(user.Id, user.Email!);

        return Task.FromResult(new AuthResponseDto(
            TokenType: "Bearer",
            AccessToken: accessToken,
            ExpiresIn: _jwtTokenGenerator.GetAccessTokenExpiryInSeconds(),
            RefreshToken: refreshToken
        ));
    }
    
    private static string GenerateRandomPassword()
    {
        const string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string digits = "0123456789";
        const string specialChars = "!@#$%^&*()-_=+[]{};:,.<>?";

        var random = new Random();
        var passwordChars = new List<char>();

        // Ensure at least one digit
        passwordChars.Add(digits[random.Next(digits.Length)]);

        // Ensure at least one special character
        passwordChars.Add(specialChars[random.Next(specialChars.Length)]);

        // Fill remaining characters with random letters/digits/specials
        string allChars = letters + digits + specialChars;
        int remainingLength = 10 - passwordChars.Count;

        for (int i = 0; i < remainingLength; i++)
        {
            passwordChars.Add(allChars[random.Next(allChars.Length)]);
        }

        // Shuffle to randomize the character positions
        return new string(passwordChars.OrderBy(_ => random.Next()).ToArray());
    }
}
