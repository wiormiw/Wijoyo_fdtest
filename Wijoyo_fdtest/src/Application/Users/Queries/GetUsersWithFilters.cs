using Wijoyo_fdtest.Application.Common.Interfaces;
using Wijoyo_fdtest.Application.Common.Models;
using Wijoyo_fdtest.Application.Common.Security;
using Wijoyo_fdtest.Domain.Constants;

namespace Wijoyo_fdtest.Application.Users.Queries;

[Authorize(Roles = Roles.User)]
public record GetUsersWithFiltersQuery : IRequest<PaginatedList<UserDetailByEmailStatusDto>>
{
    public string? Search { get; init; }
    public string? EmailStatus { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetUsersWithFiltershHandler : IRequestHandler<GetUsersWithFiltersQuery, PaginatedList<UserDetailByEmailStatusDto>>
{
    private readonly IIdentityService _identityService;

    public GetUsersWithFiltershHandler(IUser user, IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<PaginatedList<UserDetailByEmailStatusDto>> Handle(GetUsersWithFiltersQuery request, CancellationToken cancellationToken)
    {
        return await _identityService.GetUsersDetailsAsync(request.Search, request.EmailStatus, request.PageNumber, request.PageSize, cancellationToken);
    }
}
