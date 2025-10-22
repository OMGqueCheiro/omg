using OMG.Core.Entities;

namespace OMG.Domain.Contracts.Service;

public interface IClienteService
{
    Task<Cliente> Get(int id);
}
