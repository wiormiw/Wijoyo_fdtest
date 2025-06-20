using Ardalis.GuardClauses;
using Wijoyo_fdtest.Application.Common.Interfaces;
using Wijoyo_fdtest.Application.Common.Security;
using Wijoyo_fdtest.Domain.Constants;

namespace Wijoyo_fdtest.Application.Books.Queries;

[Authorize(Roles = Roles.User)]
public record GetBookDetailQuery : IRequest<BookDto>
{
    required public string BookId { get; init; }
}

public class GetBookDetailHandler : IRequestHandler<GetBookDetailQuery, BookDto>
{
    private readonly IUser _user;
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;

    public GetBookDetailHandler(IUser user, IIdentityService identityService, IApplicationDbContext context)
    {
        _user = user;
        _identityService = identityService;
        _context = context;
    }

    public async Task<BookDto> Handle(GetBookDetailQuery request, CancellationToken cancellationToken)
    {
        var userId = _user.Id;

        if (userId is null)
        {
            throw new UnauthorizedAccessException("Can't handle the request because you unauthorized.");
        }

        var book = await _context.Books
            .AsNoTracking()
            .Where(b => b.Id == Guid.Parse(request.BookId) && b.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(request.BookId, book);

        var userName = await _identityService.GetUserNameAsync(userId);

        Guard.Against.NotFound(userId, userName);

        return new BookDto
        {
            Id = book.Id.ToString(),
            Title = book.Title,
            Author = book.Author,
            Description = book.Description,
            CoverUrl = book.CoverUrl ?? string.Empty,
            Rating = book.Rating,
            UserName = userName,
            DateUploaded = book.Created
        };
    }
}
