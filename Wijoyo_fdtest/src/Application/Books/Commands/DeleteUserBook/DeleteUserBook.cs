using Ardalis.GuardClauses;
using Wijoyo_fdtest.Application.Common.Interfaces;
using Wijoyo_fdtest.Application.Common.Security;
using Wijoyo_fdtest.Domain.Constants;
using Wijoyo_fdtest.Domain.Events;

namespace Wijoyo_fdtest.Application.Books.Commands.DeleteUserBook;

[Authorize(Roles = Roles.User)]
public record DeleteUserBookCommand : IRequest<string>
{
    required public string BookId { get; set;}
};

public class DeleteUserBookCommandHandler : IRequestHandler<DeleteUserBookCommand, string>
{
    private readonly IUser _user;
    private readonly IApplicationDbContext _context;

    public DeleteUserBookCommandHandler(IUser user, IApplicationDbContext context)
    {
        _user = user;
        _context = context;
    }

    public async Task<string> Handle(DeleteUserBookCommand request, CancellationToken cancellationToken)
    {
        var userId = _user.Id;

        if (userId is null)
        {
            throw new UnauthorizedAccessException("Can't handle the request because you unauthorized.");
        }

        var book = await _context.Books
            .FirstOrDefaultAsync(x => x.Id == Guid.Parse(request.BookId) && x.UserId == userId, cancellationToken);

        Guard.Against.NotFound(request.BookId, book);

        _context.Books.Remove(book);

        book.AddDomainEvent(new BookDeletedEvent(book));
        
        await _context.SaveChangesAsync(cancellationToken);

        return book.Id.ToString();
    }
}
