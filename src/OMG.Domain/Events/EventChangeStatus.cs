using OMG.Core.Base;
using OMG.Core.Enum;

namespace OMG.Domain.Events;

public class EventChangeStatus : Event
{
    public virtual int IdPedido { get; set; }
    public virtual EPedidoStatus OldStatus { get; set; }
    public virtual EPedidoStatus NewStatus { get; set; }
    public virtual string? UsuarioNome { get; set; }
    public virtual string? UsuarioEmail { get; set; }
}
