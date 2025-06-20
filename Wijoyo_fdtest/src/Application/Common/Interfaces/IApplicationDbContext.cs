using Wijoyo_fdtest.Domain.Entities;

namespace Wijoyo_fdtest.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }
    DbSet<TodoItem> TodoItems { get; }
    DbSet<Book>  Books { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
