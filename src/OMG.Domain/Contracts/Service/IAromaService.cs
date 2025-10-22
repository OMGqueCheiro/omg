using OMG.Core.Entities;

namespace OMG.Domain.Contracts.Service;

public interface IAromaService
{
    Task<Aroma> GetFromName(string nome);
}
