﻿using Wijoyo_fdtest.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Wijoyo_fdtest.Application.TodoItems.EventHandlers;

public class TodoItemCreatedEventHandler : INotificationHandler<TodoItemCreatedEvent>
{
    private readonly ILogger<TodoItemCreatedEventHandler> _logger;

    public TodoItemCreatedEventHandler(ILogger<TodoItemCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(TodoItemCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Wijoyo_fdtest Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}