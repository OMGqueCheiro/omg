using OMG.Domain;
using OMG.Domain.Base;
using OMG.Domain.Entities;
using OMG.Domain.Handler;
using OMG.WebApp.Authentication;
using System.Net.Http.Json;

namespace OMG.WebApp.Handler;

public class ClienteHandler(AuthenticatedHttpClientFactory httpClientFactory) : IClienteHandler
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(Configuracao.HttpClientNameOMGApi);
    public async Task<Response<Cliente>> CreateOrUpdate(Cliente cliente) => cliente.Id == 0 ? await Create(cliente) : await Update(cliente);

    private async Task<Response<Cliente>> Update(Cliente cliente)
    {
        var response = await _client.PutAsJsonAsync($"api/Cliente/{cliente.Id}", cliente);

        if (response.IsSuccessStatusCode)
            return new Response<Cliente>(
                data: cliente,
                code: (int)response.StatusCode);

        return new Response<Cliente>(code: (int)response.StatusCode, message: await response.Content.ReadAsStringAsync());
    }

    private async Task<Response<Cliente>> Create(Cliente cliente)
    {
        var response = await _client.PostAsJsonAsync($"api/Cliente", cliente);

        if (response.IsSuccessStatusCode)
            return new Response<Cliente>(
                data: await response.Content.ReadFromJsonAsync<Cliente>(),
                code: (int)response.StatusCode);

        return new Response<Cliente>(code: (int)response.StatusCode, message: await response.Content.ReadAsStringAsync());
    }
}
