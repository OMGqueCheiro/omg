using OMG.Domain;
using OMG.Domain.Base;
using OMG.Domain.Entities;
using OMG.Domain.Handler;
using OMG.Domain.Mappers;
using OMG.Domain.Request;
using OMG.Domain.ViewModels;
using OMG.WebApp.Authentication;
using System.Net.Http.Json;

namespace OMG.WebApp.Handler;

public class PedidoHandler(AuthenticatedHttpClientFactory httpClientFactory) : IPedidoHandler
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(Configuracao.HttpClientNameOMGApi);

    public async Task<Response> ChangeStatus(PedidoChangeStatusRequest request)
    {
        var response = await _client.PutAsJsonAsync($"api/Pedido/ChangeStatus", request);

        if (response.IsSuccessStatusCode)
            return new Response(code: (int)response.StatusCode);

        return new Response(code: (int)response.StatusCode, message: await response.Content.ReadAsStringAsync());
    }

    public async Task<Response<IEnumerable<PedidoCard>>> GetPedidoCardList()
    {
        try
        {
            var response = await _client.GetAsync("api/View/Pedido/Card");

            if (response.IsSuccessStatusCode)
                return new Response<IEnumerable<PedidoCard>>(await response.Content.ReadFromJsonAsync<IEnumerable<PedidoCard>>(), (int)response.StatusCode);

            return new Response<IEnumerable<PedidoCard>>(code: (int)response.StatusCode, message: await response.Content.ReadAsStringAsync());
        }
        catch (Exception e)
        {

            throw;
        }
    }

    public async Task<Response<PedidoModal>> GetPedidoModal(int Id)
    {
        var response = await _client.GetAsync($"api/View/Pedido/Modal/{Id}");

        if (response.IsSuccessStatusCode)
            return new Response<PedidoModal>(await response.Content.ReadFromJsonAsync<PedidoModal>(), (int)response.StatusCode);

        return new Response<PedidoModal>(code: (int)response.StatusCode, message: await response.Content.ReadAsStringAsync());
    }

    public async Task<Response<PedidoCard>> NewPedido(NewPedidoRequest request)
    {
        var response = await _client.PostAsJsonAsync($"api/Pedido", request);

        if (response.IsSuccessStatusCode)
            return new Response<PedidoCard>(code: (int)response.StatusCode, data: (await response.Content.ReadFromJsonAsync<Pedido>()).ConvertToPedidoCard());

        return new Response<PedidoCard>(code: (int)response.StatusCode, message: await response.Content.ReadAsStringAsync());
    }
}
