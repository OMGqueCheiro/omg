using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OMG.Domain.Events;

namespace OMG.Repository.Mappings;

public class EventChangeStatusMap : IEntityTypeConfiguration<EventChangeStatus>
{
    public void Configure(EntityTypeBuilder<EventChangeStatus> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.NewStatus).IsRequired();
        builder.Property(x => x.OldStatus).HasMaxLength(250).IsRequired();
        builder.Property(x => x.DataCriacao).IsRequired();
        builder.Property(x => x.IdPedido).IsRequired();
        builder.Property(x => x.UsuarioNome).HasMaxLength(200);
        builder.Property(x => x.UsuarioEmail).HasMaxLength(200);
    }
}
