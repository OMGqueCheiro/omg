using OMG.Domain;
using OMG.Domain.Request;
using OMG.WebApp.Authentication;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace OMG.WebApp.Handler;

public class AuthHandler(IHttpClientFactory httpClientFactory, AuthenticationStateService authStateService) : IAuthHandler
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly AuthenticationStateService _authStateService = authStateService;

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var client = _httpClientFactory.CreateClient(Configuracao.HttpClientNameOMGApi);
        var response = await client.PostAsJsonAsync("/api/auth/login", request);

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
        var client = _httpClientFactory.CreateClient(Configuracao.HttpClientNameOMGApi);
        var response = await client.PostAsJsonAsync("/api/auth/register", request);

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
        var client = _httpClientFactory.CreateClient(Configuracao.HttpClientNameOMGApi);
        
        if (_authStateService.HasToken)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authStateService.Token);
        }

        var response = await client.PostAsJsonAsync("/api/auth/change-password", request);

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
        var client = _httpClientFactory.CreateClient(Configuracao.HttpClientNameOMGApi);

        if (!_authStateService.HasToken)
            return null;

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authStateService.Token);
        var response = await client.GetAsync("/api/auth/me");

        if (response.IsSuccessStatusCode)
        {
            var user = await response.Content.ReadFromJsonAsync<AuthResponse>();
            return user;
        }

        return null;
    }

    public Task LogoutAsync()
    {
        _authStateService.ClearToken();
        return Task.CompletedTask;
    }

    public Task<string?> GetStoredToken()
    {
        return Task.FromResult(_authStateService.Token);
    }
}
