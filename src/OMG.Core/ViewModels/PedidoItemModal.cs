namespace OMG.Core.ViewModels;

public class PedidoItemModal
{
    public int ItemId { get; set; }
    public int Quantidade { get; set; }
    public string Produto { get; set; } = string.Empty;
    public string Formato { get; set; } = string.Empty;
    public string Cor { get; set; } = string.Empty;
    public string Aroma { get; set; } = string.Empty;
    public string Embalagem { get; set; } = string.Empty;
}
