using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OMG.Core.Base.Contract;
using OMG.Domain.Contracts.Repository;
using OMG.Core.Entities;
using OMG.Core.Mappers;

namespace OMG.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ViewController(IRepositoryEntity<Pedido> repository, IPedidoRepository pedidoRepository) : ControllerBase
{
    private readonly IRepositoryEntity<Pedido> _repository = repository;
    private readonly IPedidoRepository _pedidoRepository = pedidoRepository;

    [HttpGet("Pedido/Card")]
    public async Task<IActionResult> GetPedidoCardList() =>
       Ok((await _pedidoRepository.GetPedidosViewHome()).Select(x => x.ConvertToPedidoCard()));



    [HttpGet("Pedido/Modal/{id}")]
    public async Task<IActionResult> GetPedidoModal(int id)
    {
        var pedido = await _repository.Get(id);

        if (pedido == null)
            return NotFound();

        return Ok(pedido.ConvertToPedidoModal());
    }
}
