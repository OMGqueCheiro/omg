using OMG.Domain.Contracts.Repository;
using OMG.Domain.Contracts.Service;
using OMG.Core.Entities;
using OMG.Core.Enum;
using OMG.Core.Request;

namespace OMG.Domain.Services;

public class PedidoService(IPedidoRepository pedidoRepository, IEventRepository eventRepository, IClienteService clienteService, ICorService corService, IAromaService aromaService, IProdutoService produtoService, IFormatoService formatoService, IEmbalagemService embalagemService) : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository = pedidoRepository;
    private readonly IEventRepository _eventRepository = eventRepository;
    private readonly IClienteService _clienteService = clienteService;
    private readonly ICorService _corService = corService;
    private readonly IAromaService _aromaService = aromaService;
    private readonly IProdutoService _produtoService = produtoService;
    private readonly IFormatoService _formatoService = formatoService;
    private readonly IEmbalagemService _embalagemService = embalagemService;

    public async Task ChangeStatus(int idPedido, EPedidoStatus newStatus, string? usuarioNome = null, string? usuarioEmail = null)
    {
        var oldstatus = await _pedidoRepository.GetPedidoStatus(idPedido);

        await _pedidoRepository.ChangePedidoStatus(idPedido, newStatus);

        await _eventRepository.EventChangeStatusPedido(idPedido, oldstatus, newStatus, usuarioNome, usuarioEmail);
    }

    public async Task<Pedido> CreateNewPedido(NewPedidoRequest newPedidoRequest)
    {
        var newPedido = new Pedido()
        {
            DataEntrega = DateOnly.FromDateTime(newPedidoRequest.DataEntrega.Value),
            Desconto = newPedidoRequest.ValorDesconto,
            IsPermuta = newPedidoRequest.IsPermuta,
            Entrada = newPedidoRequest.ValorEntrada,
            Status = EPedidoStatus.Novo,
            ValorTotal = newPedidoRequest.ValorTotal,
            PedidoItens = new List<PedidoItem>(newPedidoRequest.Itens.Count),
            Cliente = await _clienteService.Get(newPedidoRequest.ClienteId)
        };

        foreach (var item in newPedidoRequest.Itens)
            newPedido.PedidoItens.Add(new PedidoItem()
            {
                Quantidade = item.Quantidade,
                Produto = await _produtoService.GetFromDescricao(item.Produto),
                Aroma = await _aromaService.GetFromName(item.Aroma),
                Cor = await _corService.GetFromName(item.Cor),
                Formato = await _formatoService.GetFromDescricao(item.Formato),
                Embalagem = await _embalagemService.GetFromDescricao(item.Embalagem)
            });

        await _pedidoRepository.Create(newPedido);

        return newPedido;
    }


}
