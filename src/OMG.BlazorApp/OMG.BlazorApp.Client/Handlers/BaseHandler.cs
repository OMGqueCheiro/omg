using OMG.Core.Base;
using System.Net.Http.Json;

namespace OMG.BlazorApp.Client.Handlers;

/// <summary>
/// Classe base para handlers com métodos comuns para requisições HTTP
/// </summary>
public abstract class BaseHandler
{
    protected readonly HttpClient HttpClient;

    protected BaseHandler(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    /// <summary>
    /// Faz uma requisição GET e retorna um Response tipado
    /// </summary>
    protected async Task<Response<T>> GetAsync<T>(string url)
    {
        try
        {
            var response = await HttpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<T>();
                return new Response<T>(data, (int)response.StatusCode);
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            return new Response<T>(code: (int)response.StatusCode, message: errorMessage);
        }
        catch (Exception ex)
        {
            return new Response<T>(code: 500, message: $"Erro ao fazer requisição: {ex.Message}");
        }
    }

    /// <summary>
    /// Faz uma requisição POST e retorna um Response tipado
    /// </summary>
    protected async Task<Response<T>> PostAsync<T>(string url, object? data = null)
    {
        try
        {
            var response = await HttpClient.PostAsJsonAsync(url, data);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<T>();
                return new Response<T>(result, (int)response.StatusCode);
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            return new Response<T>(code: (int)response.StatusCode, message: errorMessage);
        }
        catch (Exception ex)
        {
            return new Response<T>(code: 500, message: $"Erro ao fazer requisição: {ex.Message}");
        }
    }

    /// <summary>
    /// Faz uma requisição POST sem retorno de dados tipados
    /// </summary>
    protected async Task<Response> PostAsync(string url, object? data = null)
    {
        try
        {
            var response = await HttpClient.PostAsJsonAsync(url, data);

            if (response.IsSuccessStatusCode)
            {
                return new Response(code: (int)response.StatusCode);
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            return new Response(code: (int)response.StatusCode, message: errorMessage);
        }
        catch (Exception ex)
        {
            return new Response(code: 500, message: $"Erro ao fazer requisição: {ex.Message}");
        }
    }

    /// <summary>
    /// Faz uma requisição PUT e retorna um Response tipado
    /// </summary>
    protected async Task<Response<T>> PutAsync<T>(string url, object data)
    {
        try
        {
            var response = await HttpClient.PutAsJsonAsync(url, data);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<T>();
                return new Response<T>(result, (int)response.StatusCode);
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            return new Response<T>(code: (int)response.StatusCode, message: errorMessage);
        }
        catch (Exception ex)
        {
            return new Response<T>(code: 500, message: $"Erro ao fazer requisição: {ex.Message}");
        }
    }

    /// <summary>
    /// Faz uma requisição PUT sem retorno de dados tipados
    /// </summary>
    protected async Task<Response> PutAsync(string url, object data)
    {
        try
        {
            var response = await HttpClient.PutAsJsonAsync(url, data);

            if (response.IsSuccessStatusCode)
            {
                return new Response(code: (int)response.StatusCode);
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            return new Response(code: (int)response.StatusCode, message: errorMessage);
        }
        catch (Exception ex)
        {
            return new Response(code: 500, message: $"Erro ao fazer requisição: {ex.Message}");
        }
    }

    /// <summary>
    /// Faz uma requisição DELETE
    /// </summary>
    protected async Task<Response> DeleteAsync(string url)
    {
        try
        {
            var response = await HttpClient.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return new Response(code: (int)response.StatusCode);
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            return new Response(code: (int)response.StatusCode, message: errorMessage);
        }
        catch (Exception ex)
        {
            return new Response(code: 500, message: $"Erro ao fazer requisição: {ex.Message}");
        }
    }
}
