using Language.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Language.Database.Configuration;

public class TextConfiguration : IEntityTypeConfiguration<Text>
{
    public void Configure(EntityTypeBuilder<Model.Text> builder)
    {
        builder.HasKey(i => i.Id);
        
        builder.HasMany(i => i.Dictionary).WithMany(i => i.Texts);
    }
}