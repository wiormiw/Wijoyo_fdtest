using Microsoft.AspNetCore.Http.HttpResults;
using Wijoyo_fdtest.Application.Common.Models;
using Wijoyo_fdtest.Application.Users.Queries;

namespace Wijoyo_fdtest.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetCurrentUserDetailByEmail, "/me")
            .MapGet(GetUsersWithFilters, "/filters");
    }

    public async Task<Ok<UserDetailByEmailStatusDto>> GetCurrentUserDetailByEmail(ISender sender, [AsParameters] GetCurrentUserDetailByEmailStatusQuery q)
    {
        var userDetail = await sender.Send(q);

        return TypedResults.Ok(userDetail);
    }

    public async Task<Ok<PaginatedList<UserDetailByEmailStatusDto>>> GetUsersWithFilters(ISender sender, [AsParameters] GetUsersWithFiltersQuery q)
    {
        var users = await sender.Send(q);

        return TypedResults.Ok(users);
    }
}
