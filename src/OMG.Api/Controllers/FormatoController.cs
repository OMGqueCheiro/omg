using Microsoft.AspNetCore.Mvc;
using OMG.Core.Base.Contract;
using OMG.Core.Entities;

namespace OMG.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FormatoController(IRepositoryEntity<Formato> repository) : BaseCRUDController<Formato>(repository)
{
    private readonly IRepositoryEntity<Formato> _repository = repository;

    // GET: api/Formato/search/aaa
    [HttpGet("search/{key}")]
    public async Task<IActionResult> GetSearchFormatos(string key) => Ok(await _repository.GetAll(x => x.Descricao.Contains(key)));
}
