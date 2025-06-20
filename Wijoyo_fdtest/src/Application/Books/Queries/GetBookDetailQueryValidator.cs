namespace Wijoyo_fdtest.Application.Books.Queries;

public class GetBookDetailQueryValidator : AbstractValidator<GetBookDetailQuery>
{
    public GetBookDetailQueryValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty().WithMessage("Book id need to be provided.");
    }
}
