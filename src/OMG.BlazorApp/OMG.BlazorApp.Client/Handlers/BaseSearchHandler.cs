using OMG.Core.Base;
using OMG.Core.Handler;

namespace OMG.BlazorApp.Client.Handlers;

public class BaseSearchHandler<TypeReturn> : BaseHandler, IBaseSearchHandler<TypeReturn>
{
    public BaseSearchHandler(HttpClient httpClient) : base(httpClient)
    {
    }
    
    public async Task<Response<IEnumerable<TypeReturn>>> GetAll(string UrlAction)
    {
        return await GetAsync<IEnumerable<TypeReturn>>($"api/{UrlAction}");
    }

    public async Task<Response<IEnumerable<TypeReturn>>> GetAll(string UrlAction, string Search)
    {
        return await GetAsync<IEnumerable<TypeReturn>>($"api/{UrlAction}/search/{Search}");
    }
}
