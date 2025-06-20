using Wijoyo_fdtest.Application.Common.Interfaces;
using Wijoyo_fdtest.Application.Common.Models;
using Wijoyo_fdtest.Application.Common.Security;
using Wijoyo_fdtest.Domain.Constants;

namespace Wijoyo_fdtest.Application.Books.Commands.UploadCoverImage;

[Authorize(Roles = Roles.User)]
public record UploadCoverImageCommand : IRequest<string>
{
    public FileUploadRequest File { get; init; } = null!;
}

public class UploadImageHandler : IRequestHandler<UploadCoverImageCommand, string>
{
    private readonly IStorageService _storage;

    public UploadImageHandler(IStorageService storage)
    {
        _storage = storage;
    }

    public async Task<string> Handle(UploadCoverImageCommand request, CancellationToken cancellationToken)
    {
        return await _storage.UploadFileAsync(request.File);
    }
}
