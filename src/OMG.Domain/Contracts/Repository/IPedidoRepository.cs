using OMG.Core.Entities;
using OMG.Core.Enum;

namespace OMG.Domain.Contracts.Repository;

public interface IPedidoRepository
{
    Task ChangePedidoStatus(int id, EPedidoStatus newStatus);
    Task<EPedidoStatus> GetPedidoStatus(int id);
    Task Create(Pedido pedido);
    Task<IList<Pedido>> GetPedidosViewHome(int diasExcluirProntos = 14);
}
