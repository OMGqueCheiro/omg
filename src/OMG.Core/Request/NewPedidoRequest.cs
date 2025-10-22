namespace OMG.Core.Request;

public class NewPedidoRequest
{
    public int ClienteId { get; set; } = 0;
    public IList<NewPedidoItemRequest> Itens { get; set; } = new List<NewPedidoItemRequest>();
    public decimal ValorTotal { get; set; } = 0M;
    public decimal ValorDesconto { get; set; } = 0M;
    public decimal ValorEntrada { get; set; } = 0M;
    public bool IsPermuta { get; set; } = false;
    public DateTime? DataEntrega { get; set; } = DateTime.Today.AddDays(1);
}

public record NewPedidoItemRequest(int Quantidade, string Produto, string Formato, string Cor, string Aroma, string Embalagem);
