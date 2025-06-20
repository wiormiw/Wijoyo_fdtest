using Wijoyo_fdtest.Application.Common.Models;
using Wijoyo_fdtest.Application.Users.Queries;

namespace Wijoyo_fdtest.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);
    Task<bool> IsInRoleAsync(string userId, string role);
    Task<bool> AuthorizeAsync(string userId, string policyName);
    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);
    Task<Result> DeleteUserAsync(string userId);
    Task<Result> AddToRoleAsync(string userId, string role);
    Task<UserDetailByEmailStatusDto?> GetUserDetailsByIdAsync(string userId, CancellationToken cancellationToken);
    Task<PaginatedList<UserDetailByEmailStatusDto>> GetUsersDetailsAsync(
        string? search,
        string? emailStatus,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
}
