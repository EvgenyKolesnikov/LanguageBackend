using Language.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Language.Database.Configuration;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Model.Book> builder)
    {
        builder.HasKey(i => i.Id);
        
        builder.HasMany(i => i.Users).WithMany(i => i.Books);
    }
}