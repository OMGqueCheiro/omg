using OMG.Domain;
using OMG.Domain.Base;
using OMG.Domain.Handler;
using OMG.WebApp.Authentication;
using System.Net.Http.Json;

namespace OMG.WebApp.Handler;

public class BaseSearchHandler<TypeReturn>(AuthenticatedHttpClientFactory httpClientFactory) : IBaseSearchHandler<TypeReturn>
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(Configuracao.HttpClientNameOMGApi);
    public async Task<Response<IEnumerable<TypeReturn>>> GetAll(string UrlAction)
    {
        var response = await _client.GetAsync($"api/{UrlAction}");

        if (response.IsSuccessStatusCode)
            return new Response<IEnumerable<TypeReturn>>(await response.Content.ReadFromJsonAsync<IEnumerable<TypeReturn>>(), (int)response.StatusCode);

        return new Response<IEnumerable<TypeReturn>>(code: (int)response.StatusCode, message: await response.Content.ReadAsStringAsync());
    }

    public async Task<Response<IEnumerable<TypeReturn>>> GetAll(string UrlAction, string Search)
    {
        var response = await _client.GetAsync($"api/{UrlAction}/search/{Search}");

        if (response.IsSuccessStatusCode)
            return new Response<IEnumerable<TypeReturn>>(await response.Content.ReadFromJsonAsync<IEnumerable<TypeReturn>>(), (int)response.StatusCode);

        return new Response<IEnumerable<TypeReturn>>(code: (int)response.StatusCode, message: await response.Content.ReadAsStringAsync());
    }
}
