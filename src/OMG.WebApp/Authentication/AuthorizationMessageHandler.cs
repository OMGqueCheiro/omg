using System.Net.Http.Headers;

namespace OMG.WebApp.Authentication;

public class AuthorizationMessageHandler(AuthenticationStateService authStateService, ILogger<AuthorizationMessageHandler> logger) : DelegatingHandler
{
    private readonly AuthenticationStateService _authStateService = authStateService;
    private readonly ILogger<AuthorizationMessageHandler> _logger = logger;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"AuthorizationMessageHandler - HasToken: {_authStateService.HasToken}, Token: {(_authStateService.Token != null ? "presente" : "null")}");
        
        if (_authStateService.HasToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authStateService.Token);
            _logger.LogInformation("✅ Token adicionado ao header de autorização");
        }
        else
        {
            _logger.LogWarning("⚠️ Token NÃO encontrado no AuthenticationStateService");
        }

        return base.SendAsync(request, cancellationToken);
    }
}

