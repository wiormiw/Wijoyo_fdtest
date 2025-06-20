using Ardalis.GuardClauses;
using Wijoyo_fdtest.Application.Common.Interfaces;
using Wijoyo_fdtest.Application.Common.Security;
using Wijoyo_fdtest.Domain.Constants;

namespace Wijoyo_fdtest.Application.Users.Queries;

[Authorize(Roles = Roles.User)]
public record GetCurrentUserDetailByEmailStatusQuery : IRequest<UserDetailByEmailStatusDto>;

public class GetCurrentUserDetailByEmailStatusHandler : IRequestHandler<GetCurrentUserDetailByEmailStatusQuery, UserDetailByEmailStatusDto>
{
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public GetCurrentUserDetailByEmailStatusHandler(IUser user, IIdentityService identityService)
    {
        _user = user;
        _identityService = identityService;
    }

    public async Task<UserDetailByEmailStatusDto> Handle(GetCurrentUserDetailByEmailStatusQuery request, CancellationToken cancellationToken)
    {
        var userId = _user.Id;

        if (userId is null)
        {
            throw new UnauthorizedAccessException("Can't handle the request because you unauthorized.");
        }

        var userDetails = await _identityService.GetUserDetailsByIdAsync(userId, cancellationToken);

        Guard.Against.NotFound(userId, userDetails);

        return userDetails;
    }
}
