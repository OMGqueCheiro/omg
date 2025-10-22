using Microsoft.AspNetCore.Mvc;
using OMG.Core.Base.Contract;
using OMG.Core.Entities;

namespace OMG.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmbalagemController(IRepositoryEntity<Embalagem> repository) : BaseCRUDController<Embalagem>(repository)
{
    private readonly IRepositoryEntity<Embalagem> _repository = repository;

    // GET: api/Embalagem/search/aaa
    [HttpGet("search/{key}")]
    public async Task<IActionResult> GetSearchEmbalagens(string key) => Ok(await _repository.GetAll(x => x.Descricao.Contains(key)));
}
