using Wijoyo_fdtest.Application.Common.Interfaces;
using Wijoyo_fdtest.Application.Common.Models;
using Wijoyo_fdtest.Application.Common.Security;
using Wijoyo_fdtest.Domain.Constants;

namespace Wijoyo_fdtest.Application.Books.Queries;

[Authorize(Roles = Roles.User)]
public record GetUserBooksWithFiltersQuery : IRequest<PaginatedList<UserBookDto>>
{
    public string? Author { get; init; }
    public int? Rating { get; init; }
    public string SortBy { get; init; } = "dateUploaded";
    public string SortDirection { get; init; } = "desc";
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetUserBooksWithFiltersHandler : IRequestHandler<GetUserBooksWithFiltersQuery, PaginatedList<UserBookDto>>
{
    private readonly IIdentityService _identityService;
    private readonly IUser _user;
    private readonly IApplicationDbContext _context;

    public GetUserBooksWithFiltersHandler(IIdentityService identityService, IUser user, IApplicationDbContext context)
    {
        _identityService = identityService;
        _user = user;
        _context = context;
    }

    public async Task<PaginatedList<UserBookDto>> Handle(GetUserBooksWithFiltersQuery request, CancellationToken cancellationToken)
    {
        var userId = _user.Id;

        if (userId is null)
        {
            throw new UnauthorizedAccessException("Can't handle the request because you unauthorized.");
        }

        var query = _context.Books.AsNoTracking().Where(b => b.UserId == userId);

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.Author))
        {
            query = query.Where(b => b.Author.ToLower().Contains(request.Author.ToLower()));
        }

        if (request.Rating is not null)
        {
            query = query.Where(b => b.Rating == request.Rating);
        }

        // Apply sorting
        query = query = (request.SortBy.ToLower(), request.SortDirection.ToLower()) switch
        {
            ("title", "asc") => query.OrderBy(b => b.Title),
            ("title", "desc") => query.OrderByDescending(b => b.Title),
            ("author", "asc") => query.OrderBy(b => b.Author),
            ("author", "desc") => query.OrderByDescending(b => b.Author),
            ("rating", "asc") => query.OrderBy(b => b.Rating),
            ("rating", "desc") => query.OrderByDescending(b => b.Rating),
            ("dateuploaded", "asc") => query.OrderBy(b => b.Created),
            ("dateuploaded", "desc") => query.OrderByDescending(b => b.Created),
            _ => query.OrderByDescending(b => b.Created)
        };

        // Project early and collect UserIds
        var projectedBooks = await query
            .Select(b => new
            {
                b.Id,
                b.Title,
                b.Author,
                b.Description,
                b.CoverUrl,
                b.Rating,
                b.Created,
                b.UserId
            })
            .ToListAsync(cancellationToken);

        // Paginate
        var totalItems = projectedBooks.Count;
        var itemsToTake = projectedBooks
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

    
        // Map final DTOs
        var dtos = itemsToTake.Select(b => new UserBookDto
        {
            Id = b.Id.ToString(),
            Title = b.Title,
            Author = b.Author,
            Description = b.Description,
            CoverUrl = b.CoverUrl ?? "No Cover.",
            Rating = b.Rating,
            DateUploaded = b.Created,
        }).ToList();

        return new PaginatedList<UserBookDto>(dtos, totalItems, request.PageNumber, request.PageSize);
    }
}
