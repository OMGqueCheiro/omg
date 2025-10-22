using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace OMG.WebApp.Authentication;

/// <summary>
/// Factory que cria HttpClients autenticados com o token do usuário atual.
/// Token é lido do HybridAuthenticationService - funciona sempre.
/// </summary>
public class AuthenticatedHttpClientFactory(
    IHttpClientFactory httpClientFactory,
    HybridAuthenticationService authService,
    ILogger<AuthenticatedHttpClientFactory> logger)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly HybridAuthenticationService _authService = authService;
    private readonly ILogger<AuthenticatedHttpClientFactory> _logger = logger;

    public Task<HttpClient> CreateClientAsync(string name)
    {
        return Task.FromResult(CreateClient(name));
    }
    
    /// <summary>
    /// Cria HttpClient COM autenticação (adiciona token do cookie se disponível)
    /// </summary>
    public HttpClient CreateClient(string name)
    {
        var client = _httpClientFactory.CreateClient(name);

        var token = _authService.GetToken(); // Lê do cookie HTTP
        
        if (!string.IsNullOrEmpty(token))
        {
            _logger.LogDebug("✅ Token encontrado no cookie, adicionando ao HttpClient");
            client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            _logger.LogDebug("ℹ️ Sem token - HttpClient sem autenticação");
        }

        return client;
    }

    /// <summary>
    /// Cria HttpClient SEM autenticação (para login, registro, etc)
    /// </summary>
    public HttpClient CreateUnauthenticatedClient(string name)
    {
        _logger.LogDebug("🔓 Criando client SEM autenticação");
        return _httpClientFactory.CreateClient(name);
    }
}
