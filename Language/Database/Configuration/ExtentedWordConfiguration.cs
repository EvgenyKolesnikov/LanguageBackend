using Language.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Language.Database.Configuration;

public class ExtentedWordConfiguration : IEntityTypeConfiguration<ExtentedWord>
{

    public void Configure(EntityTypeBuilder<ExtentedWord> builder)
    {
        builder.HasKey(i => i.Id);
        builder.HasIndex(i => i.Word).IsUnique();
    }
}