using OMG.Core.Base;
using OMG.Core.Entities;

namespace OMG.Core.Handler;

public interface IClienteHandler
{
    Task<Response<Cliente>> CreateOrUpdate(Cliente cliente);
}
