using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMG.Domain.Base;
using OMG.Domain.Base.Contract;
using OMG.Domain.Entities;

namespace OMG.Api.Controllers
{
    [ApiController]
    [Authorize]
    public abstract class BaseCRUDController<IEntity>(IRepositoryEntity<IEntity> repository) : ControllerBase where IEntity : Entity
    {
        private readonly IRepositoryEntity<IEntity> _repository = repository;
        [HttpGet]
        public async Task<ActionResult<IList<IEntity>>> GetEntities() => Ok(await _repository.GetAll());

        [HttpGet("{id}")]
        public async Task<ActionResult<IEntity>> GetEntity(int id)
        {
            var entity = await _repository.Get(id);

            if (entity == null)
            {
                return NotFound();
            }

            return entity;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEntity(int id, IEntity entity)
        {
            if (id != entity.Id)
            {
                return BadRequest();
            }

            try
            {
                await _repository.Update(entity);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(await _repository.Exist(id)))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Entity
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<IEntity>> PostEntity(IEntity entity)
        {
            if (entity == null)
                return BadRequest();

            entity = await _repository.Create(entity);

            return CreatedAtAction("GetEntity", new { id = entity.Id }, entity);
        }

        // DELETE: api/Entity/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntity(int id)
        {
            if (!(await _repository.Exist(id)))
            {
                return NotFound();
            }

            await _repository.Delete(id);

            return NoContent();
        }
    }
}
