using OMG.Core.Entities;
using OMG.Core.ViewModels;

namespace OMG.Domain.Mappers;

public static class PedidoMapper
{
    public static PedidoModal ConvertToPedidoModal(this Pedido pedido)
    {
        if (pedido == null) return null;

        return new()
        {
            ClienteEndereco = pedido.Cliente.Endereco,
            ClienteNome = pedido.Cliente.Nome,
            ClienteTelefone = pedido.Cliente.Telefone,
            PedidoId = pedido.Id,
            Permuta = pedido.IsPermuta,
            ValorPago = pedido.Entrada,
            ValorReceber = pedido.IsPermuta ? 0 : pedido.ValorTotal - pedido.Entrada - pedido.Desconto,
            ValorTotal = pedido.ValorTotal - pedido.Desconto,
            PedidoItens = pedido.PedidoItens.Select(x => x.ConvertToPedidoItemModal()),
            DataEntrega = pedido.DataEntrega
        };
    }

    public static PedidoCard ConvertToPedidoCard(this Pedido pedido) => new()
    {
        PedidoId = pedido.Id,
        Status = pedido.Status,
        NomeCliente = pedido.Cliente.Nome,
        TotalItens = pedido.PedidoItens.Sum(x => x.Quantidade),
        ValorTotal = pedido.ValorTotal - pedido.Desconto,
        DataEntrega = pedido.DataEntrega

    };
}
