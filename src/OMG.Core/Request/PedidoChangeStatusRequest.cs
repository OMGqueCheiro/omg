using OMG.Core.Enum;

namespace OMG.Core.Request;

public record PedidoChangeStatusRequest(int idPedido, EPedidoStatus NewStatus);