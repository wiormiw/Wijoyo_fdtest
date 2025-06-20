using System.Security.Claims;
using FluentEmail.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Wijoyo_fdtest.Application.Common.Interfaces;
using Wijoyo_fdtest.Application.Common.Models;
using Wijoyo_fdtest.Infrastructure.Identity;

namespace Wijoyo_fdtest.Web.Endpoints;

public class Auth : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(Login, "/login")
            .MapPost(Register, "/register")
            .MapPost(RefreshToken, "/refresh")
            .MapPut(ChangePassword, "/change-password")
            .MapGet(VerifyEmail, "/verify-email")
            .MapPost(SendNewPassword, "/send-new-password")
            .MapGet(OneClickPasswordReset, "/one-click-reset");
    }

    public async Task<Results<Ok<AuthResponseDto>, UnauthorizedHttpResult>> Login(
        IAuthService authService,
        [FromBody] LoginRequest request)
    {
        try
        {
            var response = await authService.LoginAsync(request);
            return TypedResults.Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Unauthorized();
        }
    }

    public async Task<Results<Ok<AuthResponseDto>, UnprocessableEntity<string>>> Register(
        IAuthService authService,
        [FromBody] RegisterRequest request, IFluentEmail  email)
    {
        try
        {
            var response = await authService.RegisterAsync(request, email);
            return TypedResults.Ok(response);
        }
        catch (ApplicationException ex)
        {
            return TypedResults.UnprocessableEntity(ex.Message);
        }
    }

    public async Task<Results<Ok<AuthResponseDto>, UnauthorizedHttpResult>> RefreshToken(
        IAuthService authService,
        [FromBody] RefreshTokenRequest request)
    {
        try
        {
            var response = await authService.RefreshTokenAsync(request.RefreshToken);
            return TypedResults.Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Unauthorized();
        }
    }
    
    public async Task<Results<NoContent, UnprocessableEntity<string>>> ChangePassword(
        IAuthService authService,
        [FromBody] ChangePasswordRequest request)
    {
        try
        {
            await authService.ChangePasswordAsync(request);
            return TypedResults.NoContent();
        }
        catch (ApplicationException ex)
        {
            return TypedResults.UnprocessableEntity(ex.Message);
        }
    }
    
    public async Task<Results<Ok, BadRequest<string>>> VerifyEmail(
        [FromServices] IVerificationTokenService tokenService,
        [FromServices] UserManager<ApplicationUser> userManager,
        [FromQuery] string token)
    {
        var principal = tokenService.ValidateEmailVerificationToken(token);
        if (principal == null)
            return TypedResults.BadRequest("Invalid or expired token.");

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var user = await userManager.FindByIdAsync(userId!);

        if (user == null)
            return TypedResults.BadRequest("User not found.");

        if (user.EmailConfirmed)
            return TypedResults.Ok(); // Already verified

        user.EmailConfirmed = true;
        var result = await userManager.UpdateAsync(user);

        return result.Succeeded
            ? TypedResults.Ok()
            : TypedResults.BadRequest("Could not confirm email.");
    }

    public async Task<Results<NoContent, NotFound<string>, BadRequest<string>>> SendNewPassword(
        [FromServices] IAuthService authService,
        [FromBody] ResetPasswordRequest request,
        IFluentEmail fluentEmail)
    {
        try
        {
            await authService.SendPasswordResetLinkAsync(request.Email, fluentEmail);
            return TypedResults.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return TypedResults.NotFound(ex.Message);
        }
        catch (ApplicationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
    
    public async Task<Results<Ok<string>, UnauthorizedHttpResult, NotFound<string>>> OneClickPasswordReset(
        [FromServices] IAuthService authService,
        [FromQuery] string token)
    {
        try
        {
            var newPassword = await authService.OneClickPasswordResetAsync(token);
            return TypedResults.Ok(newPassword);
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Unauthorized();
        }
        catch (KeyNotFoundException ex)
        {
            return TypedResults.NotFound(ex.Message);
        }
    }
}
