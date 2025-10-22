using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OMG.Core.Entities;

namespace OMG.Repository.Mappings;

public class PedidoMap : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasMany(x => x.PedidoItens).WithOne().HasForeignKey(x => x.PedidoId);
        builder.Property(x => x.Status).IsRequired();
        builder.HasOne(x => x.Cliente);
        builder.Property(x => x.DataEntrega).IsRequired();
        builder.Property(x => x.Entrada).HasDefaultValue(0.0f).HasPrecision(9, 2);
        builder.Property(x => x.ValorTotal).HasDefaultValue(0.0f).IsRequired().HasPrecision(9, 2);
        builder.Property(x => x.Desconto).HasDefaultValue(0.0f).HasPrecision(9, 2);
        builder.Property(x => x.IsPermuta).HasDefaultValue(false);

        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasIndex(i => i.IsDeleted)
            .HasFilter("IsDeleted = 0");

    }
}
