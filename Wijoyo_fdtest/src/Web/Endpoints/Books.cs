using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Wijoyo_fdtest.Application.Books.Commands;
using Wijoyo_fdtest.Application.Books.Commands.DeleteUserBook;
using Wijoyo_fdtest.Application.Books.Commands.UpdateUserBook;
using Wijoyo_fdtest.Application.Books.Commands.UploadCoverImage;
using Wijoyo_fdtest.Application.Books.Queries;
using Wijoyo_fdtest.Application.Common.Models;

namespace Wijoyo_fdtest.Web.Endpoints;

public class Books : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .RequireAuthorization()
            .MapGet(GetBooksWithFilters)
            .MapGet(GetBookDetail, "{id}")
            .MapPost(CreateUserBook)
            .MapPut(UpdateUserBook, "{id}")
            .MapDelete(DeleteUserBook, "{id}")
            .MapPost(UploadCoverImage, "/cover/upload");
    }

    public async Task<Ok<PaginatedList<UserBookDto>>> GetBooksWithFilters(ISender sender,
        [AsParameters] GetUserBooksWithFiltersQuery q)
    {
        var books = await sender.Send(q);

        return TypedResults.Ok(books);
    }
    public async Task<Ok<BookDto>> GetBookDetail(ISender sender, [FromRoute] string id)
    {
        var book = await sender.Send(new GetBookDetailQuery { BookId = id });

        return TypedResults.Ok(book);
    }

    public async Task<Created<string>> CreateUserBook(ISender sender, [FromBody] CreateUserBookCommand request)
    {
        var id = await sender.Send(request);

        return TypedResults.Created($"/{nameof(Books)}/{id}", id.ToString());
    }

    public async Task<Results<Ok<string>, BadRequest>> UpdateUserBook(ISender sender, [FromRoute] string id, [FromBody] UpdateUserBookCommand request)
    {
        if (id != request.BookId) return TypedResults.BadRequest();

        var bookId = await sender.Send(request);

        return TypedResults.Ok(bookId);
    }

    public async Task<Results<Ok<string>, BadRequest>> DeleteUserBook(ISender sender, [FromRoute] string id, [FromBody] DeleteUserBookCommand request)
    {
        if (id != request.BookId) return TypedResults.BadRequest();

        var bookId = await sender.Send(request);

        return TypedResults.Ok(bookId);
    }

    public async Task<Results<Ok<FileResponseDto>, BadRequest<string>>> UploadCoverImage(
        ISender sender,
        IFormFile file)
    {
        if (file.Length == 0)
        {
            return TypedResults.BadRequest("No file provided or file is empty.");
        }

        using var stream = file.OpenReadStream();

        var command = new UploadCoverImageCommand
        {
            File = new FileUploadRequest
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                Content = stream
            }
        };

        var result = await sender.Send(command);
        return TypedResults.Ok<FileResponseDto>(new FileResponseDto{
            Url = result
        });
    }
}
