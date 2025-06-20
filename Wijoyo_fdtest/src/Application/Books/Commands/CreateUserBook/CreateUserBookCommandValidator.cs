using Wijoyo_fdtest.Application.Common.Interfaces;

namespace Wijoyo_fdtest.Application.Books.Commands.CreateUserBook;

public class CreateUserBookCommandValidator : AbstractValidator<CreateUserBookCommand>
{
    private readonly IApplicationDbContext _context;
    public CreateUserBookCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Book title is required.")
            .MaximumLength(255).WithMessage("Book title exceeded maximum value of 255 characters.")
            .MustAsync(BeUniqueTitle).WithMessage("Book title needs to be unique.")
            .Must(x => x?.Trim().Length > 0).WithMessage("Book title cannot be whitespace only.");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Book author is required.")
            .MaximumLength(255).WithMessage("Book author exceeded maximum value of 255 characters.")
            .MustAsync(BeUniqueTitle).WithMessage("Book author needs to be unique.")
            .Must(x => x?.Trim().Length > 0).WithMessage("Book author cannot be whitespace only.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Book description is required.")
            .MaximumLength(255).WithMessage("Book description exceeded maximum value of 255 characters.")
            .Must(x => x?.Trim().Length > 0).WithMessage("Book description cannot be whitespace only.");;

        RuleFor(x => x.CoverUrl)
            .NotEmpty().WithMessage("Book cover needs to be provided.")
            .Must(x => x?.Trim().Length > 0).WithMessage("Book cover url is invalid.");;

        RuleFor(x => x.Rating)
            .NotNull().WithMessage("Book rating needs to be provided.")
            .InclusiveBetween(1, 5).WithMessage("Book rating must be between 1 and 5.");
    }

    private async Task<bool> BeUniqueTitle(string title, CancellationToken cancellationToken)
    {
        return !await _context.Books.AnyAsync(x => x.Title == title, cancellationToken);
    }
}
