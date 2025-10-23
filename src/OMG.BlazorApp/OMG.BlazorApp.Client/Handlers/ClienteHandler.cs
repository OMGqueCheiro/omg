using OMG.Core.Base;
using OMG.Core.Entities;
using OMG.Core.Handler;
using System.Net.Http.Json;

namespace OMG.BlazorApp.Client.Handlers;

/// <summary>
/// Handler de exemplo para demonstrar como fazer chamadas à API.
/// O HttpClient faz requisições ao servidor Blazor que funciona como proxy reverso (YARP).
/// O servidor encaminha as requisições /api/* para a OMG.Api.
/// Fluxo: Client (WASM) -> Servidor Blazor (YARP) -> OMG.Api
/// </summary>
public class ClienteHandler : IClienteHandler
{
    private readonly HttpClient _httpClient;

    public ClienteHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<Response<Cliente>> CreateOrUpdate(Cliente cliente) 
        => cliente.Id == 0 ? await Create(cliente) : await Update(cliente);

    private async Task<Response<Cliente>> Update(Cliente cliente)
    {
        // Chamada será /api/Cliente/{id} -> YARP encaminha para OMG.Api
        var response = await _httpClient.PutAsJsonAsync($"api/Cliente/{cliente.Id}", cliente);

        if (response.IsSuccessStatusCode)
            return new Response<Cliente>(
                data: cliente,
                code: (int)response.StatusCode);

        return new Response<Cliente>(
            code: (int)response.StatusCode, 
            message: await response.Content.ReadAsStringAsync());
    }

    private async Task<Response<Cliente>> Create(Cliente cliente)
    {
        // Chamada será /api/Cliente -> YARP encaminha para OMG.Api
        var response = await _httpClient.PostAsJsonAsync($"api/Cliente", cliente);

        if (response.IsSuccessStatusCode)
            return new Response<Cliente>(
                data: await response.Content.ReadFromJsonAsync<Cliente>(),
                code: (int)response.StatusCode);

        return new Response<Cliente>(
            code: (int)response.StatusCode, 
            message: await response.Content.ReadAsStringAsync());
    }
}
