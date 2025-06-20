using Wijoyo_fdtest.Application.Common.Models;

namespace Wijoyo_fdtest.Application.Common.Interfaces;

public interface IStorageService
{
    Task<string> UploadFileAsync(FileUploadRequest file);
}
