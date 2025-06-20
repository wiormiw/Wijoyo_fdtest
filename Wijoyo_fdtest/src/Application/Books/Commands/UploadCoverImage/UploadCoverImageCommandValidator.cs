namespace Wijoyo_fdtest.Application.Books.Commands.UploadCoverImage;

public class UploadCoverImageCommandValidator : AbstractValidator<UploadCoverImageCommand>
{
    private readonly string[] allowedMimeTypes = { "image/jpeg", "image/png" };
    public UploadCoverImageCommandValidator()
    {
        RuleFor(x => x.File).NotNull();

        RuleFor(x => x.File.ContentType)
            .NotEmpty()
            .Must(ct => allowedMimeTypes.Contains(ct))
            .WithMessage("Only JPEG and PNG images are allowed.");

        RuleFor(x => x.File.Content)
            .NotNull()
            .Must(stream => stream.Length > 0)
            .WithMessage("File content cannot be empty.");
    }
}
