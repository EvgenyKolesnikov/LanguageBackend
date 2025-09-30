using Language.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Language.Database.Configuration;

public class BaseWordConfiguration : IEntityTypeConfiguration<Model.Word>
{
    public void Configure(EntityTypeBuilder<Model.Word> builder)
    {
        builder.HasKey(i => i.Id);
        builder.HasIndex(i => i.WordText).IsUnique();
        
        
        builder.HasMany(i => i.Users).WithMany(i => i.Dictionary);
        builder.HasOne(i => i.ParentWord).WithMany(i => i.ChildrenWords)
            .HasForeignKey(i => i.ParentWordId);
    }
}