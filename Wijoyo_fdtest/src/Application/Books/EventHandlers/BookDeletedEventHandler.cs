using Microsoft.Extensions.Logging;
using Wijoyo_fdtest.Domain.Events;

namespace Microsoft.Extensions.DependencyInjection.Books.EventHandlers;

public class BookDeletedEventHandler : INotificationHandler<BookDeletedEvent>
{
    private readonly ILogger<BookCreatedEventHandler> _logger;

    public BookDeletedEventHandler(ILogger<BookCreatedEventHandler> logger)
    {
        _logger = logger;
    }
    
    public Task Handle(BookDeletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Wijoyo_fdtest Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
