using Language.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Language.Database.Configuration;

public class BaseWordConfiguration : IEntityTypeConfiguration<Model.BaseWord>
{
    public void Configure(EntityTypeBuilder<Model.BaseWord> builder)
    {
        builder.HasKey(i => i.Id);
        builder.HasIndex(i => i.Word).IsUnique();
        
        
        builder.HasMany(i => i.Users).WithMany(i => i.Dictionary);
        builder.HasMany(i => i.ExtentedWords).WithOne(i => i.BaseWord);
    }
}