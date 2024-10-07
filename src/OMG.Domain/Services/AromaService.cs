﻿using OMG.Domain.Base.Contract;
using OMG.Domain.Contracts.Service;
using OMG.Domain.Entities;

namespace OMG.Domain.Services;

internal class AromaService(IRepositoryEntity<Aroma> repository) : IAromaService
{
    private readonly IRepositoryEntity<Aroma> _repository = repository;
    public async Task<Aroma> GetFromName(string nome)
    {
        var aroma = await _repository.Get(x => x.Nome.ToLower().Trim() == nome.ToLower().Trim());

        if (aroma == null) return await _repository.Create(new Aroma { Nome = nome});

        return aroma;
    }
}
