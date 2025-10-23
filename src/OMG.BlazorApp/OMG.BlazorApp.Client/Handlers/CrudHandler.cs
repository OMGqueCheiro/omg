using OMG.Core.Base;
using OMG.Core.Handler;

namespace OMG.BlazorApp.Client.Handlers;

/// <summary>
/// Handler genérico para operações CRUD
/// </summary>
public class CrudHandler<TEntity> : BaseHandler, ICrudHandler<TEntity> where TEntity : Entity
{
    private readonly string _controllerName;

    public CrudHandler(HttpClient httpClient, string controllerName) : base(httpClient)
    {
        _controllerName = controllerName;
    }

    public async Task<Response<IEnumerable<TEntity>>> GetAllAsync()
    {
        return await GetAsync<IEnumerable<TEntity>>($"api/{_controllerName}");
    }

    public async Task<Response<TEntity>> GetByIdAsync(int id)
    {
        return await GetAsync<TEntity>($"api/{_controllerName}/{id}");
    }

    public async Task<Response<TEntity>> CreateAsync(TEntity entity)
    {
        return await PostAsync<TEntity>($"api/{_controllerName}", entity);
    }

    public async Task<Response<TEntity>> UpdateAsync(TEntity entity)
    {
        return await PutAsync<TEntity>($"api/{_controllerName}/{entity.Id}", entity);
    }

    public async Task<Response> DeleteAsync(int id)
    {
        return await DeleteAsync($"api/{_controllerName}/{id}");
    }
}
