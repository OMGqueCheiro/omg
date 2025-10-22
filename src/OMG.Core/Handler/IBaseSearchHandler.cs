using OMG.Core.Base;

namespace OMG.Core.Handler;


public interface IBaseSearchHandler<TypeReturn>
{
    Task<Response<IEnumerable<TypeReturn>>> GetAll(string UrlAction);
    Task<Response<IEnumerable<TypeReturn>>> GetAll(string UrlAction, string Search);
}
