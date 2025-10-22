using Microsoft.AspNetCore.Mvc;
using OMG.Core.Base.Contract;
using OMG.Core.Entities;

namespace OMG.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProdutoController(IRepositoryEntity<Produto> repository) : BaseCRUDController<Produto>(repository)
{
    private readonly IRepositoryEntity<Produto> _repository = repository;

    // GET: api/Produto/search/aaa
    [HttpGet("search/{key}")]
    public async Task<IActionResult> GetSearchProdutos(string key) => Ok(await _repository.GetAll(x => x.Descricao.Contains(key)));
}
