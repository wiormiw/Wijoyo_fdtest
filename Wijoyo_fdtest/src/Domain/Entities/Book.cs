namespace Wijoyo_fdtest.Domain.Entities;

public class Book : BaseAuditableEntity
{
    public string Title { get; set; } = default!;
    public string Author { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? CoverUrl { get; set; } = default!;
    public int Rating { get; set; } = default!;
    public string UserId { get; set; } = default!;
}

