using Wijoyo_fdtest.Application.Common.Interfaces;
using Wijoyo_fdtest.Domain.Entities;
using Wijoyo_fdtest.Domain.Events;

namespace Wijoyo_fdtest.Application.TodoItems.Commands.CreateTodoItem;

public record CreateTodoItemCommand : IRequest<string>
{
    required public string ListId { get; init; }

    public string? Title { get; init; }
}

public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, string>
{
    private readonly IApplicationDbContext _context;

    public CreateTodoItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoItem
        {
            ListId = Guid.Parse(request.ListId),
            Title = request.Title,
            Done = false
        };

        entity.AddDomainEvent(new TodoItemCreatedEvent(entity));

        _context.TodoItems.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id.ToString();
    }
}
