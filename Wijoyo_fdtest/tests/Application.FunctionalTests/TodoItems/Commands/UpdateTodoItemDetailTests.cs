using Wijoyo_fdtest.Application.TodoItems.Commands.CreateTodoItem;
using Wijoyo_fdtest.Application.TodoItems.Commands.UpdateTodoItem;
using Wijoyo_fdtest.Application.TodoItems.Commands.UpdateTodoItemDetail;
using Wijoyo_fdtest.Application.TodoLists.Commands.CreateTodoList;
using Wijoyo_fdtest.Domain.Entities;
using Wijoyo_fdtest.Domain.Enums;

namespace Wijoyo_fdtest.Application.FunctionalTests.TodoItems.Commands;

using static Testing;

public class UpdateTodoItemDetailTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new UpdateTodoItemCommand { Id = "6B29FC40-CA47-1067-B31D-00DD010662DA", Title = "New Title" };
        await FluentActions.Invoking(() => SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldUpdateTodoItem()
    {
        var userId = await RunAsDefaultUserAsync();

        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var itemId = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId.ToString(),
            Title = "New Item"
        });

        var command = new UpdateTodoItemDetailCommand
        {
            Id = itemId,
            ListId = listId.ToString(),
            Note = "This is the note.",
            Priority = PriorityLevel.High
        };

        await SendAsync(command);

        var item = await FindAsync<TodoItem>(itemId);

        item.Should().NotBeNull();
        item!.ListId.Should().Be(Guid.Parse(command.ListId));
        item.Note.Should().Be(command.Note);
        item.Priority.Should().Be(command.Priority);
        item.LastModifiedBy.Should().NotBeNull();
        item.LastModifiedBy.Should().Be(userId);
        item.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
