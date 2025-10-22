using Microsoft.AspNetCore.Mvc;
using OMG.Core.Base.Contract;
using OMG.Core.Entities;

namespace OMG.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AromaController(IRepositoryEntity<Aroma> repository) : BaseCRUDController<Aroma>(repository)
{
    private readonly IRepositoryEntity<Aroma> _repository = repository;

    // GET: api/Aroma/search/aaa
    [HttpGet("search/{key}")]
    public async Task<IActionResult> GetSearchAromas(string key) => Ok(await _repository.GetAll(x => x.Nome.Contains(key)));

}
