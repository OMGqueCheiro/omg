using Microsoft.EntityFrameworkCore;
using OMG.Core.Base;
using OMG.Core.Base.Contract;
using System.Linq.Expressions;

namespace OMG.Repository.Repositories;

public class EntityRepository<IEntity>(OMGDbContext dbContext) : IRepositoryEntity<IEntity> where IEntity : Entity
{
    private readonly OMGDbContext _dbContext = dbContext;

    public async Task<IEntity> Create(IEntity entity)
    {
        await _dbContext.Set<IEntity>().AddAsync(entity);

        await _dbContext.SaveChangesAsync();

        return entity;
    }

    public async Task<bool> Delete(int id)
    {
        var entity = await _dbContext.Set<IEntity>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (entity == null) return false;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<IEntity> Get(int id) => await _dbContext.Set<IEntity>().FindAsync(id);

    public async Task<IEntity> Get(Expression<Func<IEntity, bool>> predicate) => await _dbContext.Set<IEntity>().FirstOrDefaultAsync(predicate);

    public async Task<IList<IEntity>> GetAll(Expression<Func<IEntity, bool>> predicate = null) => predicate == null
            ? await _dbContext.Set<IEntity>().ToListAsync()
            : (IList<IEntity>)await _dbContext.Set<IEntity>().Where(predicate).ToListAsync();

    public async Task<IEntity> Update(IEntity entity)
    {
        dbContext.Set<IEntity>().Update(entity);

        await _dbContext.SaveChangesAsync();

        return entity;
    }

    public async Task<bool> Exist(int id) => await dbContext.Set<IEntity>().AnyAsync(e => e.Id == id);

}
