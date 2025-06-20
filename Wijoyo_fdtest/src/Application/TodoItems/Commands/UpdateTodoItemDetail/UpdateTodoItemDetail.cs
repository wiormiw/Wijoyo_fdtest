using Wijoyo_fdtest.Application.Common.Interfaces;
using Wijoyo_fdtest.Domain.Enums;

namespace Wijoyo_fdtest.Application.TodoItems.Commands.UpdateTodoItemDetail;

public record UpdateTodoItemDetailCommand : IRequest
{
    required public string Id { get; init; }

    required public string ListId { get; init; }

    public PriorityLevel Priority { get; init; }

    public string? Note { get; init; }
}

public class UpdateTodoItemDetailCommandHandler : IRequestHandler<UpdateTodoItemDetailCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateTodoItemDetailCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateTodoItemDetailCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoItems
            .FindAsync(new object[] { Guid.Parse(request.Id) }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        entity.ListId = Guid.Parse(request.ListId);
        entity.Priority = request.Priority;
        entity.Note = request.Note;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
