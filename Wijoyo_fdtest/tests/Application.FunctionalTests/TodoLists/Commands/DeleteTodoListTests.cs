using Wijoyo_fdtest.Application.TodoLists.Commands.CreateTodoList;
using Wijoyo_fdtest.Application.TodoLists.Commands.DeleteTodoList;
using Wijoyo_fdtest.Domain.Entities;

namespace Wijoyo_fdtest.Application.FunctionalTests.TodoLists.Commands;

using static Testing;

public class DeleteTodoListTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidTodoListId()
    {
        var command = new DeleteTodoListCommand("6B29FC40-CA47-1067-B31D-00DD010662DA");
        await FluentActions.Invoking(() => SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldDeleteTodoList()
    {
        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        await SendAsync(new DeleteTodoListCommand(listId));

        var list = await FindAsync<TodoList>(listId);

        list.Should().BeNull();
    }
}
