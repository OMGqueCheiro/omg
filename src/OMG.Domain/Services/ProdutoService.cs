using OMG.Core.Base.Contract;
using OMG.Domain.Contracts.Service;
using OMG.Core.Entities;

namespace OMG.Domain.Services;

public class ProdutoService(IRepositoryEntity<Produto> repository) : IProdutoService
{
    private readonly IRepositoryEntity<Produto> _repository = repository;
    public async Task<Produto> GetFromDescricao(string descricao)
    {
        var produto = await _repository.Get(x => x.Descricao.ToLower().Trim() == descricao.ToLower().Trim());

        if (produto == null) return await _repository.Create(new Produto { Descricao = descricao });

        return produto;
    }
}
