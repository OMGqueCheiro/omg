using OMG.Core.Base;
using OMG.Core.Enum;

namespace OMG.Core.Entities;

public class Pedido : Entity
{
    public required EPedidoStatus Status { get; set; } = EPedidoStatus.Novo;

    public required virtual Cliente Cliente { get; set; }

    public required virtual IList<PedidoItem> PedidoItens { get; set; }

    public decimal ValorTotal { get; set; } = 0m;
    public decimal Desconto { get; set; } = 0m;
    public decimal Entrada { get; set; } = 0m;
    public bool IsPermuta { get; set; } = false;
    public DateOnly DataEntrega { get; set; } = new DateOnly();
}
