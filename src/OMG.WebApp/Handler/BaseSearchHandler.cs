using OMG.Core.Base;
using OMG.Core.Handler;
using System.Net.Http.Json;

namespace OMG.WebApp.Handler;

public class BaseSearchHandler<TypeReturn> : IBaseSearchHandler<TypeReturn>
{
    private readonly HttpClient _httpClient;

    public BaseSearchHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<Response<IEnumerable<TypeReturn>>> GetAll(string UrlAction)
    {
        // HttpClient já configurado via DI
        var response = await _httpClient.GetAsync($"api/{UrlAction}");

        if (response.IsSuccessStatusCode)
            return new Response<IEnumerable<TypeReturn>>(await response.Content.ReadFromJsonAsync<IEnumerable<TypeReturn>>(), (int)response.StatusCode);

        return new Response<IEnumerable<TypeReturn>>(code: (int)response.StatusCode, message: await response.Content.ReadAsStringAsync());
    }

    public async Task<Response<IEnumerable<TypeReturn>>> GetAll(string UrlAction, string Search)
    {
        // HttpClient já configurado via DI
        var response = await _httpClient.GetAsync($"api/{UrlAction}/search/{Search}");

        if (response.IsSuccessStatusCode)
            return new Response<IEnumerable<TypeReturn>>(await response.Content.ReadFromJsonAsync<IEnumerable<TypeReturn>>(), (int)response.StatusCode);

        return new Response<IEnumerable<TypeReturn>>(code: (int)response.StatusCode, message: await response.Content.ReadAsStringAsync());
    }
}
