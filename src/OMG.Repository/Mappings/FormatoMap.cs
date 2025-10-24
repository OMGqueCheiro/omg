using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OMG.Core.Entities;

namespace OMG.Repository.Mappings;

public class FormatoMap : IEntityTypeConfiguration<Formato>
{
    public void Configure(EntityTypeBuilder<Formato> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Descricao).HasMaxLength(250).IsRequired();

        builder.HasIndex(i => i.Descricao);

        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasIndex(i => i.IsDeleted)
            .HasFilter("\"IsDeleted\" = false");
    }
}
