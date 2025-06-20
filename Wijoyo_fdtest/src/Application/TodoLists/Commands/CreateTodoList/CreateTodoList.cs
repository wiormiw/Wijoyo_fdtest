using Wijoyo_fdtest.Application.Common.Interfaces;
using Wijoyo_fdtest.Domain.Entities;

namespace Wijoyo_fdtest.Application.TodoLists.Commands.CreateTodoList;

public record CreateTodoListCommand : IRequest<string>
{
    public string? Title { get; init; }
}

public class CreateTodoListCommandHandler : IRequestHandler<CreateTodoListCommand, string>
{
    private readonly IApplicationDbContext _context;

    public CreateTodoListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(CreateTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoList();

        entity.Title = request.Title;

        _context.TodoLists.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id.ToString();
    }
}
