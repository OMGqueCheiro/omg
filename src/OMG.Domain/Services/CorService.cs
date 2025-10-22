using OMG.Core.Base.Contract;
using OMG.Domain.Contracts.Service;
using OMG.Core.Entities;

namespace OMG.Domain.Services;

public class CorService(IRepositoryEntity<Cor> repository) : ICorService
{
    private readonly IRepositoryEntity<Cor> _corRepository = repository;
    public async Task<Cor> GetFromName(string nome)
    {
        var cor = await _corRepository.Get(x => x.Nome.ToLower().Trim() == nome.ToLower().Trim());

        if (cor == null) return await _corRepository.Create(new Cor { Nome = nome });

        return cor;
    }
}
