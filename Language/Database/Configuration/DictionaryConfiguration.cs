using Language.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Language.Database.Configuration;

public class DictionaryConfiguration : IEntityTypeConfiguration<Model.Dictionary>
{
    public void Configure(EntityTypeBuilder<Model.Dictionary> builder)
    {
        builder.HasKey(i => i.Id);
        builder.HasIndex(i => i.Word).IsUnique();
        
        
        builder.HasMany(i => i.Users).WithMany(i => i.Dictionary);
    }
}