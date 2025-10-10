using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace OMG.WebApp.Authentication;

/// <summary>
/// Factory que cria HttpClients autenticados com o token do usuário atual.
/// Resolve o problema de escopo do AuthorizationMessageHandler.
/// </summary>
public class AuthenticatedHttpClientFactory(
    IHttpClientFactory httpClientFactory,
    AuthenticationStateService authStateService,
    ILogger<AuthenticatedHttpClientFactory> logger)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly AuthenticationStateService _authStateService = authStateService;
    private readonly ILogger<AuthenticatedHttpClientFactory> _logger = logger;

    public HttpClient CreateClient(string name)
    {
        var client = _httpClientFactory.CreateClient(name);

        if (_authStateService.HasToken)
        {
            _logger.LogInformation("✅ Token encontrado, adicionando ao HttpClient");
            client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _authStateService.Token);
        }
        else
        {
            _logger.LogWarning("⚠️ Token NÃO encontrado ao criar HttpClient");
        }

        return client;
    }
}
