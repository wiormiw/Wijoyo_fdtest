using Wijoyo_fdtest.Application.Common.Interfaces;
using Wijoyo_fdtest.Application.Common.Security;
using Wijoyo_fdtest.Domain.Constants;
using Wijoyo_fdtest.Domain.Entities;
using Wijoyo_fdtest.Domain.Events;

namespace Wijoyo_fdtest.Application.Books.Commands;

[Authorize(Roles = Roles.User)]
public record CreateUserBookCommand : IRequest<string>
{
    required public string Title { get; init; }
    required public string Author { get; init; }
    required public string Description { get; init; }
    required public string CoverUrl { get; init; }
    required public int Rating { get; init; }
}

public class CreateUserBookCommandHandler : IRequestHandler<CreateUserBookCommand, string>
{
    private readonly IUser _user;
    private readonly IApplicationDbContext _context;

    public CreateUserBookCommandHandler(IUser user, IApplicationDbContext context)
    {
        _user = user;
        _context = context;
    }

    public async Task<string> Handle(CreateUserBookCommand request, CancellationToken cancellationToken)
    {
        var userId = _user.Id;

        if (userId is null)
        {
            throw new UnauthorizedAccessException("Can't handle the request because you unauthorized.");
        }

        var newBook = new Book
        {
            Title = request.Title,
            Author = request.Author,
            Description = request.Description,
            CoverUrl = request.CoverUrl,
            Rating = request.Rating,
            UserId = userId,
        };

        newBook.AddDomainEvent(new BookCreatedEvent(newBook));
        
        _context.Books.Add(newBook);

        await _context.SaveChangesAsync(cancellationToken);

        return newBook.Id.ToString();
    }
}
