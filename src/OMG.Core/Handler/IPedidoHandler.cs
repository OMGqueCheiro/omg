using OMG.Core.Base;
using OMG.Core.Request;
using OMG.Core.ViewModels;

namespace OMG.Core.Handler;

public interface IPedidoHandler
{
    Task<Response<IEnumerable<PedidoCard>>> GetPedidoCardList();
    Task<Response<PedidoModal>> GetPedidoModal(int Id);
    Task<Response> ChangeStatus(PedidoChangeStatusRequest request);
    Task<Response<PedidoCard>> NewPedido(NewPedidoRequest request);
}
