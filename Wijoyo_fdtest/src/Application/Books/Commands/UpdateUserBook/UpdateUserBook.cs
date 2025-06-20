using Ardalis.GuardClauses;
using Wijoyo_fdtest.Application.Common.Interfaces;
using Wijoyo_fdtest.Application.Common.Security;
using Wijoyo_fdtest.Domain.Constants;

namespace Wijoyo_fdtest.Application.Books.Commands.UpdateUserBook;

[Authorize(Roles = Roles.User)]
public record UpdateUserBookCommand : IRequest<string>
{
    required public string BookId { get; set;}
    public string? Title { get; init; }
    public string? Author { get; init; }
    public string? Description { get; init; }
    public string? CoverUrl { get; init; }
    public int? Rating { get; init; }
}

public class UpdateUserBookCommandHandler : IRequestHandler<UpdateUserBookCommand, string>
{
    private readonly IUser _user;
    private readonly IApplicationDbContext _context;

    public UpdateUserBookCommandHandler(IUser user, IApplicationDbContext context)
    {
        _user = user;
        _context = context;
    }

    public async Task<string> Handle(UpdateUserBookCommand request, CancellationToken cancellationToken)
    {
        var userId = _user.Id;

        if (userId is null)
        {
            throw new UnauthorizedAccessException("Can't handle the request because you unauthorized.");
        }

        var book = await _context.Books
            .FirstOrDefaultAsync(x => x.Id == Guid.Parse(request.BookId) && x.UserId == userId, cancellationToken);


        Guard.Against.NotFound(request.BookId, book);

        if (request.Title is not null)
        {
            book.Title = request.Title;
        }

        if (request.Author is not null)
        {
            book.Author = request.Author;
        }

        if (request.Description is not null)
        {
            book.Description = request.Description;
        }

        if (request.CoverUrl is not null)
        {
            book.CoverUrl = request.CoverUrl;
        }

        if (request.Rating is not null)
        {
            book.Rating = (int)request.Rating;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return book.Id.ToString();
    }
}
