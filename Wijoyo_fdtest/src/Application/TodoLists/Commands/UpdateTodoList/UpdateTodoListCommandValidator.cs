using Wijoyo_fdtest.Application.Common.Interfaces;

namespace Wijoyo_fdtest.Application.TodoLists.Commands.UpdateTodoList;

public class UpdateTodoListCommandValidator : AbstractValidator<UpdateTodoListCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateTodoListCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Title)
            .NotEmpty()
            .MaximumLength(200)
            .MustAsync(BeUniqueTitle)
            .WithMessage("'{PropertyName}' must be unique.")
            .WithErrorCode("Unique");
    }

    public async Task<bool> BeUniqueTitle(UpdateTodoListCommand model, string title,
        CancellationToken cancellationToken)
    {
        return !await _context.TodoLists
            .Where(l => l.Id != Guid.Parse(model.Id))
            .AnyAsync(l => l.Title == title, cancellationToken);
    }
}
