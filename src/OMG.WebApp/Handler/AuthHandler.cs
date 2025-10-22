using OMG.Core.Handler;
using OMG.Core.Request;
using System.Net.Http.Json;

namespace OMG.WebApp.Handler;

/// <summary>
/// Handler para operações de autenticação via API (Blazor Server SSR)
/// Durante SSR, não temos acesso ao localStorage do navegador.
/// Métodos que dependem de token armazenado retornam null.
/// </summary>
public class AuthHandler : IAuthHandler
{
    private readonly HttpClient _httpClient;

    public AuthHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);

        if (response.IsSuccessStatusCode)
        {
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (authResponse?.Success == true)
            {
                return authResponse;
            }
        }

        var errorResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return errorResponse ?? new AuthResponse(false, Message: "Erro ao fazer login.");
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request);

        if (response.IsSuccessStatusCode)
        {
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
            return authResponse ?? new AuthResponse(false, Message: "Erro ao registrar.");
        }

        var errorResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return errorResponse ?? new AuthResponse(false, Message: "Erro ao registrar usuário.");
    }

    public async Task<AuthResponse> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/change-password", request);

        if (response.IsSuccessStatusCode)
        {
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
            return authResponse ?? new AuthResponse(false, Message: "Erro ao alterar senha.");
        }

        var errorResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return errorResponse ?? new AuthResponse(false, Message: "Erro ao alterar senha.");
    }

    public async Task<AuthResponse?> GetCurrentUserAsync()
    {
        // Durante SSR no servidor, não temos acesso ao token do localStorage
        // Este método só funciona após hidratação WASM no Client
        return await Task.FromResult<AuthResponse?>(null);
    }

    public async Task LogoutAsync()
    {
        // Durante SSR, não fazemos nada
        // O logout real acontece no Client após hidratação WASM
        await Task.CompletedTask;
    }

    public async Task<string?> GetStoredToken()
    {
        // Durante SSR, não temos acesso ao localStorage
        return await Task.FromResult<string?>(null);
    }
}
