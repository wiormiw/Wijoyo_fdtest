namespace Wijoyo_fdtest.Application.Books.Queries;

public class UserBookDto
{
    public required string Id { get; set; }
    public required string Title { get; init; }
    public required string Author { get; init; }
    public required string Description { get; init; }
    public required string CoverUrl { get; init; }
    public required int Rating { get; init; }
    public required DateTimeOffset DateUploaded { get; init; }
}

