using System.Security.Claims;
using Wijoyo_fdtest.Application.Common.Interfaces;

namespace Wijoyo_fdtest.Web.Services;

public class CurrentUser : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue("userId")
                        ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");
    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue("email");
}