using OMG.Core.Base;

namespace OMG.Core.Entities;

public class PedidoItem : Entity
{
    public int PedidoId { get; set; }

    public required virtual Produto Produto { get; set; }
    public required virtual Formato Formato { get; set; }
    public required virtual Cor Cor { get; set; }
    public required virtual Aroma Aroma { get; set; }
    public required virtual Embalagem Embalagem { get; set; }

    public int Quantidade { get; set; }
}
