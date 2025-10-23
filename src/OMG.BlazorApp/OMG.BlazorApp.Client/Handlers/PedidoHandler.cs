using OMG.Core.Base;
using OMG.Core.Entities;
using OMG.Core.Handler;
using OMG.Core.Mappers;
using OMG.Core.Request;
using OMG.Core.ViewModels;
using System.Net.Http.Json;

namespace OMG.BlazorApp.Client.Handlers;

public class PedidoHandler : IPedidoHandler
{
    private readonly HttpClient _httpClient;

    public PedidoHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Response> ChangeStatus(PedidoChangeStatusRequest request)
    {
        // HttpClient já configurado via DI
        var response = await _httpClient.PutAsJsonAsync($"api/Pedido/ChangeStatus", request);

        if (response.IsSuccessStatusCode)
            return new Response(code: (int)response.StatusCode);

        return new Response(code: (int)response.StatusCode, message: await response.Content.ReadAsStringAsync());
    }

    public async Task<Response<IEnumerable<PedidoCard>>> GetPedidoCardList()
    {
        try
        {
            // HttpClient já configurado via DI
            var response = await _httpClient.GetAsync("api/View/Pedido/Card");

            if (response.IsSuccessStatusCode)
                return new Response<IEnumerable<PedidoCard>>(await response.Content.ReadFromJsonAsync<IEnumerable<PedidoCard>>(), (int)response.StatusCode);

            return new Response<IEnumerable<PedidoCard>>(code: (int)response.StatusCode, message: await response.Content.ReadAsStringAsync());
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Response<PedidoModal>> GetPedidoModal(int Id)
    {
        // HttpClient já configurado via DI
        var response = await _httpClient.GetAsync($"api/View/Pedido/Modal/{Id}");

        if (response.IsSuccessStatusCode)
            return new Response<PedidoModal>(await response.Content.ReadFromJsonAsync<PedidoModal>(), (int)response.StatusCode);

        return new Response<PedidoModal>(code: (int)response.StatusCode, message: await response.Content.ReadAsStringAsync());
    }

    public async Task<Response<PedidoCard>> NewPedido(NewPedidoRequest request)
    {
        // HttpClient já configurado via DI
        var response = await _httpClient.PostAsJsonAsync($"api/Pedido", request);

        if (response.IsSuccessStatusCode)
            return new Response<PedidoCard>(code: (int)response.StatusCode, data: (await response.Content.ReadFromJsonAsync<Pedido>()).ConvertToPedidoCard());

        return new Response<PedidoCard>(code: (int)response.StatusCode, message: await response.Content.ReadAsStringAsync());
    }
}
