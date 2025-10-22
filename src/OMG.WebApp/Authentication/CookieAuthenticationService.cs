using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;

namespace OMG.WebApp.Authentication;

/// <summary>
/// Serviço de autenticação que usa Cookies HTTP para persistir o token JWT.
/// Cookies são a solução mais robusta para Blazor Server.
/// </summary>
public class CookieAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CookieAuthenticationService> _logger;
    private const string TokenCookieName = "OMG.AuthToken";
    private string? _cachedToken;

    public CookieAuthenticationService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<CookieAuthenticationService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Obtém o token do cookie HTTP.
    /// Funciona durante prerendering pois cookies são enviados no request HTTP.
    /// </summary>
    public string? GetToken()
    {
        // Retorna cache se já carregado
        if (_cachedToken != null)
            return _cachedToken;

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            _logger.LogWarning("⚠️ HttpContext não disponível");
            return null;
        }

        if (httpContext.Request.Cookies.TryGetValue(TokenCookieName, out var token))
        {
            _cachedToken = token;
            _logger.LogDebug("✅ Token recuperado do cookie (tamanho: {Length})", token.Length);
            return token;
        }

        _logger.LogDebug("ℹ️ Nenhum token encontrado nos cookies");
        return null;
    }

    /// <summary>
    /// Salva o token em um cookie HTTP.
    /// Cookie é HttpOnly e Secure para segurança.
    /// </summary>
    public void SetToken(string token)
    {
        _logger.LogInformation("💾 Salvando token no cookie (tamanho: {Length})", token.Length);
        
        _cachedToken = token;

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            _logger.LogError("❌ HttpContext não disponível ao salvar token");
            return;
        }

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,  // Não acessível via JavaScript (segurança contra XSS)
            Secure = true,    // Apenas HTTPS
            SameSite = SameSiteMode.Strict,  // Proteção contra CSRF
            Expires = DateTimeOffset.UtcNow.AddHours(8),  // Expira em 8 horas
            Path = "/"
        };

        httpContext.Response.Cookies.Append(TokenCookieName, token, cookieOptions);
        _logger.LogInformation("✅ Token salvo no cookie HTTP");
    }

    /// <summary>
    /// Remove o token do cookie.
    /// </summary>
    public void ClearToken()
    {
        _logger.LogInformation("🗑️ Removendo token do cookie");
        
        _cachedToken = null;

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            _logger.LogWarning("⚠️ HttpContext não disponível ao limpar token");
            return;
        }

        httpContext.Response.Cookies.Delete(TokenCookieName);
        _logger.LogInformation("✅ Token removido do cookie HTTP");
    }

    public bool HasToken()
    {
        return !string.IsNullOrEmpty(GetToken());
    }
}
