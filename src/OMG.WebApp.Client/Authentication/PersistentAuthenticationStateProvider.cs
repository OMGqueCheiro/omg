using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OMG.WebApp.Client.Authentication;

/// <summary>
/// AuthenticationStateProvider para o lado WASM que lê o token do localStorage
/// </summary>
public class PersistentAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;
    private const string TokenKey = "authToken";

    public PersistentAuthenticationStateProvider(
        ILocalStorageService localStorage,
        HttpClient httpClient)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        Console.WriteLine("🔍 [AUTH] GetAuthenticationStateAsync iniciado");
        string? token = null;
        
        try
        {
            Console.WriteLine("🔍 [AUTH] Tentando ler token do localStorage...");
            token = await _localStorage.GetItemAsync<string>(TokenKey);

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("🔓 [AUTH] Nenhum token encontrado - usuário anônimo");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            Console.WriteLine($"🔑 [AUTH] Token encontrado! Tamanho: {token.Length} caracteres");
            Console.WriteLine($"🔑 [AUTH] Primeiros 20 chars: {token.Substring(0, Math.Min(20, token.Length))}...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ [AUTH] Erro ao acessar localStorage: {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine($"❌ [AUTH] StackTrace: {ex.StackTrace}");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var handler = new JwtSecurityTokenHandler();
        
        try
        {
            var jwtToken = handler.ReadJwtToken(token);
            
            // Verifica se o token expirou
            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                await _localStorage.RemoveItemAsync(TokenKey);
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var claims = jwtToken.Claims.ToList();
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            // Adiciona o token ao header do HttpClient
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return new AuthenticationState(user);
        }
        catch
        {
            await _localStorage.RemoveItemAsync(TokenKey);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public async Task NotifyUserAuthentication(string token)
    {
        Console.WriteLine($"💾 [AUTH] NotifyUserAuthentication - Salvando token (tamanho: {token.Length})");
        
        try
        {
            await _localStorage.SetItemAsync(TokenKey, token);
            Console.WriteLine($"✅ [AUTH] Token salvo com sucesso no localStorage com chave '{TokenKey}'");
            
            // Verifica se realmente foi salvo
            var verificacao = await _localStorage.GetItemAsync<string>(TokenKey);
            if (verificacao == token)
            {
                Console.WriteLine("✅ [AUTH] Verificação: Token foi salvo corretamente!");
            }
            else
            {
                Console.WriteLine($"⚠️ [AUTH] AVISO: Token verificado é diferente! Salvo: {verificacao?.Length ?? 0} chars");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ [AUTH] ERRO ao salvar token: {ex.GetType().Name}: {ex.Message}");
            throw;
        }
        
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var claims = jwtToken.Claims.ToList();
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        Console.WriteLine($"👤 [AUTH] Claims parseadas: {claims.Count} claims");
        Console.WriteLine($"👤 [AUTH] Email: {claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "N/A"}");

        // Adiciona o token ao header do HttpClient
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        Console.WriteLine("📢 [AUTH] Notificando mudança de estado de autenticação...");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        Console.WriteLine("✅ [AUTH] NotifyUserAuthentication concluído!");
    }

    public async Task NotifyUserLogout()
    {
        await _localStorage.RemoveItemAsync(TokenKey);
        
        // Remove o token do header do HttpClient
        _httpClient.DefaultRequestHeaders.Authorization = null;

        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _localStorage.GetItemAsync<string>(TokenKey);
    }
}
