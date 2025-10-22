using System.Linq.Expressions;

namespace OMG.Core.Base.Contract;

public interface IRepositoryEntity<IEntity> where IEntity : Entity
{
    Task<IList<IEntity>> GetAll(Expression<Func<IEntity, bool>> predicate = null);
    Task<IEntity> Get(int id);
    Task<IEntity> Get(Expression<Func<IEntity, bool>> predicate);
    Task<IEntity> Create(IEntity entity);
    Task<IEntity> Update(IEntity entity);
    Task<bool> Delete(int id);
    Task<bool> Exist(int id);
}
public interface IRepositoryEvent<IEvent> where IEvent : Event
{
    Task<IList<IEvent>> GetAll(Expression<Func<IEvent, bool>> predicate = null);
    Task<IEvent> Get(int id);
    Task<IEvent> Get(Expression<Func<IEvent, bool>> predicate);
    Task<IEvent> Create(IEvent entity);
}
