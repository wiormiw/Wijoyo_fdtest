using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wijoyo_fdtest.Domain.Entities;
using Wijoyo_fdtest.Infrastructure.Identity;

namespace Wijoyo_fdtest.Infrastructure.Data.Configurations;

public class BookConfiguration
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.Property(b => b.Title).IsRequired();
        builder.Property(b => b.Author).IsRequired();
        builder.Property(b => b.Description).IsRequired();
        builder.Property(b => b.Rating).HasDefaultValue(1).IsRequired();

        builder.HasOne<ApplicationUser>()
            .WithMany(u => u.Books)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
