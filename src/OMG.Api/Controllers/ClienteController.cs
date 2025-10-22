using Microsoft.AspNetCore.Mvc;
using OMG.Core.Base.Contract;
using OMG.Core.Entities;

namespace OMG.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClienteController(IRepositoryEntity<Cliente> repository) : BaseCRUDController<Cliente>(repository)
{
    private readonly IRepositoryEntity<Cliente> _repository = repository;

    // GET: api/Cliente/search/aaa
    [HttpGet("search/{key}")]
    public async Task<IActionResult> GetSearchClientes(string key) => Ok(await _repository.GetAll(x => x.Nome.Contains(key)));
}
