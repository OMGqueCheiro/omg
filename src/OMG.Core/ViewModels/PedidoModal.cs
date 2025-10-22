namespace OMG.Core.ViewModels;

public class PedidoModal
{
    public int PedidoId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public string ClienteTelefone { get; set; } = string.Empty;
    public string ClienteEndereco { get; set; } = string.Empty;
    public IEnumerable<PedidoItemModal> PedidoItens { get; set; } = Array.Empty<PedidoItemModal>();
    public decimal ValorTotal { get; set; }
    public decimal ValorPago { get; set; }
    public decimal ValorReceber { get; set; }
    public bool Permuta { get; set; } = false;
    public DateOnly DataEntrega { get; set; }
}
