using OMG.Core.Base;
using OMG.Core.Entities;
using OMG.Core.Handler;
using OMG.Core.Mappers;
using OMG.Core.Request;
using OMG.Core.ViewModels;
using System.Net.Http.Json;

namespace OMG.BlazorApp.Client.Handlers;

public class PedidoHandler : BaseHandler, IPedidoHandler
{
    public PedidoHandler(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<Response> ChangeStatus(PedidoChangeStatusRequest request)
    {
        return await PutAsync("api/Pedido/ChangeStatus", request);
    }

    public async Task<Response<IEnumerable<PedidoCard>>> GetPedidoCardList()
    {
        return await GetAsync<IEnumerable<PedidoCard>>("api/View/Pedido/Card");
    }

    public async Task<Response<PedidoModal>> GetPedidoModal(int Id)
    {
        return await GetAsync<PedidoModal>($"api/View/Pedido/Modal/{Id}");
    }

    public async Task<Response<PedidoCard>> NewPedido(NewPedidoRequest request)
    {
        var response = await PostAsync<Pedido>("api/Pedido", request);
        
        if (response.IsSuccess && response.Data != null)
        {
            return new Response<PedidoCard>(
                code: response.Code,
                data: response.Data.ConvertToPedidoCard());
        }

        return new Response<PedidoCard>(code: response.Code, message: response.Message);
    }
}
