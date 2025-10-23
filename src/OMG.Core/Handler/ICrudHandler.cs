using OMG.Core.Base;

namespace OMG.Core.Handler;

/// <summary>
/// Interface genérica para operações CRUD com suporte a soft delete
/// </summary>
public interface ICrudHandler<TEntity> where TEntity : Entity
{
    Task<Response<IEnumerable<TEntity>>> GetAllAsync();
    Task<Response<TEntity>> GetByIdAsync(int id);
    Task<Response<TEntity>> CreateAsync(TEntity entity);
    Task<Response<TEntity>> UpdateAsync(TEntity entity);
    Task<Response> DeleteAsync(int id);
}
