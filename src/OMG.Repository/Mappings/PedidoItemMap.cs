using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OMG.Core.Entities;

namespace OMG.Repository.Mappings;

public class PedidoItemMap : IEntityTypeConfiguration<PedidoItem>
{
    public void Configure(EntityTypeBuilder<PedidoItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Cor);
        builder.HasOne(x => x.Formato);
        builder.HasOne(x => x.Produto);
        builder.HasOne(x => x.Aroma);

        builder.Property(x => x.Quantidade);

        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasIndex(i => i.IsDeleted)
            .HasFilter("\"IsDeleted\" = false");

    }
}
