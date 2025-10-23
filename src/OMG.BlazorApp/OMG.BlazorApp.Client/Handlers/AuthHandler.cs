using OMG.Core.Handler;
using OMG.Core.Request;
using OMG.BlazorApp.Client.Authentication;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace OMG.BlazorApp.Client.Handlers;

/// <summary>
/// Handler para operações de autenticação via API (Blazor WASM)
/// </summary>
public class AuthHandler : BaseHandler, IAuthHandler
{
    private readonly JwtAuthenticationStateProvider _authStateProvider;

    public AuthHandler(
        HttpClient httpClient,
        AuthenticationStateProvider authStateProvider) : base(httpClient)
    {
        _authStateProvider = (JwtAuthenticationStateProvider)authStateProvider;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await HttpClient.PostAsJsonAsync("/api/auth/login", request);

            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (authResponse?.Success == true && !string.IsNullOrEmpty(authResponse.Token))
                {
                    // Notifica o AuthenticationStateProvider e salva o token
                    await _authStateProvider.NotifyUserAuthentication(authResponse.Token);
                    return authResponse;
                }
            }

            var errorResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
            return errorResponse ?? new AuthResponse(false, Message: "Erro ao fazer login.");
        }
        catch (Exception ex)
        {
            return new AuthResponse(false, Message: $"Erro ao fazer login: {ex.Message}");
        }
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await HttpClient.PostAsJsonAsync("/api/auth/register", request);

            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                return authResponse ?? new AuthResponse(false, Message: "Erro ao registrar.");
            }

            var errorResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
            return errorResponse ?? new AuthResponse(false, Message: "Erro ao registrar usuário.");
        }
        catch (Exception ex)
        {
            return new AuthResponse(false, Message: $"Erro ao registrar: {ex.Message}");
        }
    }

    public async Task<AuthResponse> ChangePasswordAsync(ChangePasswordRequest request)
    {
        try
        {
            var response = await HttpClient.PostAsJsonAsync("/api/auth/change-password", request);

            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                return authResponse ?? new AuthResponse(false, Message: "Erro ao alterar senha.");
            }

            var errorResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
            return errorResponse ?? new AuthResponse(false, Message: "Erro ao alterar senha.");
        }
        catch (Exception ex)
        {
            return new AuthResponse(false, Message: $"Erro ao alterar senha: {ex.Message}");
        }
    }

    public async Task<AuthResponse?> GetCurrentUserAsync()
    {
        try
        {
            var token = await _authStateProvider.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return null;

            var response = await HttpClient.GetAsync("/api/auth/me");

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<AuthResponse>();
                return user;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task LogoutAsync()
    {
        await _authStateProvider.NotifyUserLogout();
    }

    public async Task<string?> GetStoredToken()
    {
        return await _authStateProvider.GetTokenAsync();
    }
}
