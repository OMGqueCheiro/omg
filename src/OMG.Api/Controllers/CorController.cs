using Microsoft.AspNetCore.Mvc;
using OMG.Core.Base.Contract;
using OMG.Core.Entities;

namespace OMG.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CorController(IRepositoryEntity<Cor> repository) : BaseCRUDController<Cor>(repository)
{
    private readonly IRepositoryEntity<Cor> _repository = repository;

    // GET: api/Cor/search/aaa
    [HttpGet("search/{key}")]
    public async Task<IActionResult> GetSearchCores(string key) => Ok(await _repository.GetAll(x => x.Nome.Contains(key)));
}
