using Wijoyo_fdtest.Application.Common.Interfaces;

namespace Wijoyo_fdtest.Application.Books.Commands.UpdateUserBook;

public class UpdateUserBookCommandValidator : AbstractValidator<UpdateUserBookCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateUserBookCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.BookId)
            .NotEmpty().WithMessage("Books Id need to be provided for update.");

        When(x => !string.IsNullOrWhiteSpace(x.Title), () =>
        {
            RuleFor(x => x.Title!)
                .MaximumLength(255).WithMessage("Book title exceeded maximum value of 255 characters.")
                .MustAsync(BeUniqueTitle).WithMessage("Book title needs to be unique.")
                .Must(x => x!.Trim().Length > 0).WithMessage("Book title cannot be whitespace only.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.Author), () =>
        {
            RuleFor(x => x.Author!)
                .MaximumLength(255).WithMessage("Book author exceeded maximum value of 255 characters.")
                .Must(x => x!.Trim().Length > 0).WithMessage("Book author cannot be whitespace only.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.Description), () =>
        {
            RuleFor(x => x.Description!)
                .MaximumLength(255).WithMessage("Book description exceeded maximum value of 255 characters.")
                .Must(x => x!.Trim().Length > 0).WithMessage("Book description cannot be whitespace only.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.CoverUrl), () =>
        {
            RuleFor(x => x.CoverUrl!)
                .Must(x => x!.Trim().Length > 0).WithMessage("Book cover url is invalid.");
        });

        When(x => x.Rating.HasValue, () =>
        {
            RuleFor(x => x.Rating!.Value)
                .InclusiveBetween(1, 5).WithMessage("Book rating must be between 1 and 5.");
        });
    }

    private async Task<bool> BeUniqueTitle(UpdateUserBookCommand model, string? title, CancellationToken cancellationToken)
    {
        return !await _context.Books.AnyAsync(x =>
            x.Title == title && x.Id != Guid.Parse(model.BookId), cancellationToken);
    }
}

