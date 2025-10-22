using OMG.Core.Enum;

namespace OMG.Domain.Contracts.Repository;

public interface IEventRepository
{
    Task EventChangeStatusPedido(int idPedido, EPedidoStatus oldStatus, EPedidoStatus newStatus, string? usuarioNome = null, string? usuarioEmail = null);
}
