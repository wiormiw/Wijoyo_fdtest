using Wijoyo_fdtest.Application.TodoItems.Commands.CreateTodoItem;
using Wijoyo_fdtest.Application.TodoItems.Commands.DeleteTodoItem;
using Wijoyo_fdtest.Application.TodoLists.Commands.CreateTodoList;
using Wijoyo_fdtest.Domain.Entities;

namespace Wijoyo_fdtest.Application.FunctionalTests.TodoItems.Commands;

using static Testing;

public class DeleteTodoItemTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new DeleteTodoItemCommand("6B29FC40-CA47-1067-B31D-00DD010662DA");

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldDeleteTodoItem()
    {
        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var itemId = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId.ToString(),
            Title = "New Item"
        });

        await SendAsync(new DeleteTodoItemCommand(itemId));

        var item = await FindAsync<TodoItem>(itemId);

        item.Should().BeNull();
    }
}
