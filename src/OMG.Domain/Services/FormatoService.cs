using OMG.Core.Base.Contract;
using OMG.Domain.Contracts.Service;
using OMG.Core.Entities;

namespace OMG.Domain.Services;

public class FormatoService(IRepositoryEntity<Formato> repository) : IFormatoService
{
    private readonly IRepositoryEntity<Formato> _repository = repository;
    public async Task<Formato> GetFromDescricao(string descricao)
    {
        var formato = await _repository.Get(x => x.Descricao.ToLower().Trim() == descricao.ToLower().Trim());

        if (formato == null) return await _repository.Create(new Formato { Descricao = descricao });

        return formato;
    }
}
