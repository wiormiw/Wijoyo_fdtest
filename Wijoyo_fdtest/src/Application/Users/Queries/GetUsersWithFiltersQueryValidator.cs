namespace Wijoyo_fdtest.Application.Users.Queries;

public class GetUsersWithFiltersQueryValidator : AbstractValidator<GetUsersWithFiltersQuery>
{
    public GetUsersWithFiltersQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100");

        When(x => !string.IsNullOrWhiteSpace(x.EmailStatus), () =>
        {
            RuleFor(x => x.EmailStatus)
                .Must(status => status!.ToLower() is "verified" or "unverified")
                .WithMessage("EmailStatus must be 'verified' or 'unverified'");
        });

        RuleFor(x => x.Search)
            .MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.Search))
            .WithMessage("Search cannot be longer than 100 characters.");
    }
}
