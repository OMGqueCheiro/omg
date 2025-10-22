using OMG.Core.Entities;
using OMG.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OMG.Domain.Queries;

public static class PedidoQuery
{
    public static Expression<Func<Pedido, bool>> GetPedidoExcludePedidoStatus(EPedidoStatus ExcludeStatus)
    {
        return x => x.Status != ExcludeStatus;
    }

    public static Expression<Func<Pedido, bool>> GetPedidoWherePedidoStatusEqualAndDataEntregaMenorQue(EPedidoStatus Status, int DataEntregaMenorQue)
    {
        return x => x.Status == Status && 
            x.DataEntrega >= DateOnly.FromDateTime(DateTime.Now).AddDays(-DataEntregaMenorQue);
    }
}
