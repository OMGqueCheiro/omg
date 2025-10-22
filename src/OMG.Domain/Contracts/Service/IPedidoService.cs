using OMG.Core.Entities;
using OMG.Core.Enum;
using OMG.Core.Request;

namespace OMG.Domain.Contracts.Service;

public interface IPedidoService
{
    Task ChangeStatus(int idPedido, EPedidoStatus newStatus, string? usuarioNome = null, string? usuarioEmail = null);
    Task<Pedido> CreateNewPedido(NewPedidoRequest newPedidoRequest);
}
