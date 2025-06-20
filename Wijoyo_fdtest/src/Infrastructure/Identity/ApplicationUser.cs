using Microsoft.AspNetCore.Identity;
using Wijoyo_fdtest.Domain.Entities;

namespace Wijoyo_fdtest.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
