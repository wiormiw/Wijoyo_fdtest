using Microsoft.Extensions.Logging;
using Wijoyo_fdtest.Domain.Events;

namespace Microsoft.Extensions.DependencyInjection.Books.EventHandlers;

public class BookCreatedEventHandler : INotificationHandler<BookCreatedEvent>
{
    private readonly ILogger<BookCreatedEventHandler> _logger;

    public BookCreatedEventHandler(ILogger<BookCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(BookCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Wijoyo_fdtest Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
