using Wijoyo_fdtest.Application.Common.Models;
using Microsoft.AspNetCore.Identity;

namespace Wijoyo_fdtest.Infrastructure.Identity;

public static class IdentityResultExtensions
{
    public static Result ToApplicationResult(this IdentityResult result)
    {
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(result.Errors.Select(e => e.Description));
    }
}