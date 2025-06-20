using Wijoyo_fdtest.Application.Common.Interfaces;
using Wijoyo_fdtest.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wijoyo_fdtest.Application.Users.Queries;

namespace Wijoyo_fdtest.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user?.UserName;
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = userName,
        };

        var result = await _userManager.CreateAsync(user, password);

        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null ? await DeleteUserAsync(user) : Result.Success();
    }

    public async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }
    
    public async Task<Result> AddToRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return Result.Failure(["User not found."]);
        }

        if (!await _roleManager.RoleExistsAsync(role))
        {
            return Result.Failure(["User role does not exist."]);
        }

        var result = await _userManager.AddToRoleAsync(user, role);

        return result.ToApplicationResult();
    }
    
    public async Task<UserDetailByEmailStatusDto?> GetUserDetailsByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            return null;
        }

        return new UserDetailByEmailStatusDto
        {
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            EmailStatus = user.EmailConfirmed ? "Verified" : "Unverified"
        };
    }

    public async Task<PaginatedList<UserDetailByEmailStatusDto>> GetUsersDetailsAsync(
        string? search,
        string? emailStatus,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _userManager.Users.AsNoTracking();

        // Filter by email verification status
        if (!string.IsNullOrWhiteSpace(emailStatus))
        {
            query = emailStatus.Trim().ToLower() switch
            {
                "verified" => query.Where(u => u.EmailConfirmed),
                "unverified" => query.Where(u => !u.EmailConfirmed),
                _ => query
            };
        }

        // Search by username or email
        if (!string.IsNullOrWhiteSpace(search))
        {
            var lowered = search.Trim().ToLower();
            query = query.Where(u =>
                u.UserName!.ToLower().Contains(lowered) ||
                u.Email!.ToLower().Contains(lowered));
        }

        // Project to DTO and paginate
        var projectedQuery = query
            .OrderBy(u => u.UserName ?? "")
            .Select(u => new UserDetailByEmailStatusDto
            {
                UserName = u.UserName ?? string.Empty,
                Email = u.Email ?? string.Empty,
                EmailStatus = u.EmailConfirmed ? "Verified" : "Unverified"
            });

        return await PaginatedList<UserDetailByEmailStatusDto>.CreateAsync(
            projectedQuery, pageNumber, pageSize, cancellationToken);
    }
}
