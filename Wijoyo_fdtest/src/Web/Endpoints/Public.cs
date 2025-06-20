using Microsoft.AspNetCore.Http.HttpResults;
using Wijoyo_fdtest.Application.Books.Queries;
using Wijoyo_fdtest.Application.Common.Models;

namespace Wijoyo_fdtest.Web.Endpoints;

public class Public : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(PublicGetBooksWithFilters, "/Books");
    }

    public async Task<Ok<PaginatedList<BookDto>>> PublicGetBooksWithFilters(ISender sender, [AsParameters] GetBooksWithFiltersQuery q)
    {
        var books = await sender.Send(q);

        return TypedResults.Ok(books);
    }
}
