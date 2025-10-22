using Microsoft.EntityFrameworkCore;
using OMG.Core.Base.Contract;
using OMG.Domain.Contracts.Repository;
using OMG.Core.Entities;
using OMG.Core.Enum;
using OMG.Domain.Queries;

namespace OMG.Repository.Repositories;

public class PedidoRepository(OMGDbContext context, IRepositoryEntity<Pedido> repository) : IPedidoRepository
{
    private readonly OMGDbContext _context = context;
    private readonly IRepositoryEntity<Pedido> _repository = repository;

    public async Task ChangePedidoStatus(int id, EPedidoStatus newStatus)
    {
        if (!await _repository.Exist(id)) throw new Exception($"Pedido ({id}) não encontrado");

        var pedido = await _repository.Get(id);

        pedido.Status = newStatus;

        await _repository.Update(pedido);
    }

    public async Task Create(Pedido pedido) => await _repository.Create(pedido);

    public async Task<EPedidoStatus> GetPedidoStatus(int id)
    {
        if (!await _repository.Exist(id)) throw new Exception($"Pedido ({id}) não encontrado");

        return (await _repository.Get(id)).Status;
    }


    public async Task<IList<Pedido>> GetPedidosViewHome(int diasExcluirProntos = 14) => await PedidoViewHomeQuery(diasExcluirProntos).ToListAsync();
    private IQueryable<Pedido> PedidoViewHomeQuery(int diasExcluirProntos = 14)
        => _context.Pedidos.Where(PedidoQuery.GetPedidoExcludePedidoStatus(EPedidoStatus.Entregue))
                           .Union(_context.Pedidos.Where(PedidoQuery.GetPedidoWherePedidoStatusEqualAndDataEntregaMenorQue(EPedidoStatus.Entregue, diasExcluirProntos)));
}
