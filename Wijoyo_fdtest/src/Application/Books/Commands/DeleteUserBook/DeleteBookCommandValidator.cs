namespace Wijoyo_fdtest.Application.Books.Commands.DeleteUserBook;

public class DeleteBookCommandValidator : AbstractValidator<DeleteUserBookCommand>
{
    public DeleteBookCommandValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty().WithMessage("Books Id need to be provided for update.");
    }
}
