﻿using OMG.Domain.Contracts.Repository;
using OMG.Domain.Contracts.Service;
using OMG.Domain.Entities;
using OMG.Domain.Enum;
using OMG.Domain.Request;

namespace OMG.Domain.Services;

internal class PedidoService(IPedidoRepository pedidoRepository, IEventRepository eventRepository, IClienteService clienteService, ICorService corService, IAromaService aromaService, IProdutoService produtoService, IFormatoService formatoService) : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository = pedidoRepository;
    private readonly IEventRepository _eventRepository = eventRepository;
    private readonly IClienteService _clienteService = clienteService;
    private readonly ICorService _corService = corService;
    private readonly IAromaService _aromaService = aromaService;
    private readonly IProdutoService _produtoService = produtoService;
    private readonly IFormatoService _formatoService = formatoService;

    public async Task ChangeStatus(int idPedido, EPedidoStatus newStatus)
    {
        var oldstatus = await _pedidoRepository.GetPedidoStatus(idPedido);

        await _pedidoRepository.ChangePedidoStatus(idPedido, newStatus);

        await _eventRepository.EventChangeStatusPedido(idPedido, oldstatus, newStatus);
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
                Formato = await _formatoService.GetFromDescricao(item.Formato)
            });

        await _pedidoRepository.Create(newPedido);

        return newPedido;
    }


}
