using OMG.Core.Enum;

namespace OMG.Core.ViewModels;

public class PedidoCard
{
    public int PedidoId { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public int TotalItens { get; set; }
    public DateOnly DataEntrega { get; set; } = new DateOnly();
    public decimal ValorTotal { get; set; }
    public EPedidoStatus Status { get; set; }
}
