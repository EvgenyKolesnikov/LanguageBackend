using Language.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Language.Database.Configuration;

public class WordProperitiesConfiguration : IEntityTypeConfiguration<WordProperties>
{
    public void Configure(EntityTypeBuilder<WordProperties> builder)
    {
        builder.HasKey(i => i.Id);
        
        builder.HasOne(i => i.Word).WithMany(i => i.Properties);
    }
}