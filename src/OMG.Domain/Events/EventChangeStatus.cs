﻿using OMG.Domain.Base;
using OMG.Domain.Enum;

namespace OMG.Domain.Events;

public class EventChangeStatus : Event
{
    public virtual int IdPedido { get; set; }
    public virtual EPedidoStatus OldStatus { get; set; }
    public virtual EPedidoStatus NewStatus { get; set; }

}
