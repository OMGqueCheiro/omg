using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OMG.BlazorApp.Client.Authentication;

/// <summary>
/// AuthenticationStateProvider para o lado WASM que gerencia autenticação via JWT no localStorage
/// </summary>
public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;
    private const string TokenKey = "authToken";

    public JwtAuthenticationStateProvider(
        ILocalStorageService localStorage,
        HttpClient httpClient)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string? token = null;
        
        try
        {
            token = await _localStorage.GetItemAsync<string>(TokenKey);

            if (string.IsNullOrEmpty(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }
        catch (Exception)
        {
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

            // Adiciona o token ao header do HttpClient ANTES de criar a identidade
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Cria a identidade com os claims do JWT
            var identity = CreateIdentityFromToken(jwtToken);
            var user = new ClaimsPrincipal(identity);

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
        await _localStorage.SetItemAsync(TokenKey, token);
        
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Adiciona o token ao header do HttpClient
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Cria a identidade com os claims do JWT
        var identity = CreateIdentityFromToken(jwtToken);
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
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

    /// <summary>
    /// Cria uma ClaimsIdentity a partir do token JWT, mapeando claims customizados
    /// </summary>
    private ClaimsIdentity CreateIdentityFromToken(JwtSecurityToken jwtToken)
    {
        var claims = new List<Claim>();

        // Adiciona todos os claims do token
        claims.AddRange(jwtToken.Claims);

        // Mapeia claims customizados para claims padrão do ASP.NET
        var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email || c.Type == "email");
        var nomeClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nome");
        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId");

        // Adiciona ClaimTypes.Name para que Identity.Name funcione
        if (emailClaim != null)
        {
            // Remove duplicatas se existirem
            claims.RemoveAll(c => c.Type == ClaimTypes.Name);
            claims.Add(new Claim(ClaimTypes.Name, emailClaim.Value));
        }

        // Adiciona ClaimTypes.Email se não existir
        if (emailClaim != null && !claims.Any(c => c.Type == ClaimTypes.Email))
        {
            claims.Add(new Claim(ClaimTypes.Email, emailClaim.Value));
        }

        // Adiciona ClaimTypes.NameIdentifier para UserId
        if (userIdClaim != null)
        {
            claims.RemoveAll(c => c.Type == ClaimTypes.NameIdentifier);
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userIdClaim.Value));
        }

        // Adiciona claim de nome completo
        if (nomeClaim != null && !string.IsNullOrEmpty(nomeClaim.Value))
        {
            claims.RemoveAll(c => c.Type == "nome_completo");
            claims.Add(new Claim("nome_completo", nomeClaim.Value));
        }

        return new ClaimsIdentity(claims, "jwt");
    }
}

