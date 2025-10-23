using OMG.Core.Base;
using OMG.Core.Entities;
using OMG.Core.Handler;

namespace OMG.BlazorApp.Client.Handlers;

/// <summary>
/// Handler de exemplo para demonstrar como fazer chamadas à API.
/// O HttpClient faz requisições ao servidor Blazor que funciona como proxy reverso (YARP).
/// O servidor encaminha as requisições /api/* para a OMG.Api.
/// Fluxo: Client (WASM) -> Servidor Blazor (YARP) -> OMG.Api
/// </summary>
public class ClienteHandler : BaseHandler, IClienteHandler
{
    public ClienteHandler(HttpClient httpClient) : base(httpClient)
    {
    }
    
    public async Task<Response<Cliente>> CreateOrUpdate(Cliente cliente) 
        => cliente.Id == 0 ? await Create(cliente) : await Update(cliente);

    private async Task<Response<Cliente>> Update(Cliente cliente)
    {
        return await PutAsync<Cliente>($"api/Cliente/{cliente.Id}", cliente);
    }

    private async Task<Response<Cliente>> Create(Cliente cliente)
    {
        return await PostAsync<Cliente>("api/Cliente", cliente);
    }
}
