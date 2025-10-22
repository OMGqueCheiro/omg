using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;

namespace OMG.WebApp.Authentication;

/// <summary>
/// Serviço híbrido que combina HTTP Cookies (quando possível) com ProtectedLocalStorage
/// Esta é a solução definitiva para Blazor Server
/// </summary>
public class HybridAuthenticationService
{
    private readonly ProtectedLocalStorage _localStorage;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<HybridAuthenticationService> _logger;
    private const string TokenKey = "auth_token";
    private const string CookieName = "OMG.AuthToken";
    private string? _tokenCache;
    private bool _isInitialized = false;

    public HybridAuthenticationService(
        ProtectedLocalStorage localStorage,
        IHttpContextAccessor httpContextAccessor,
        ILogger<HybridAuthenticationService> logger)
    {
        _localStorage = localStorage;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Obtém o token (síncrono - usa cache em memória)
    /// </summary>
    public string? GetToken()
    {
        return _tokenCache;
    }

    /// <summary>
    /// Inicializa carregando token do storage ou cookie
    /// DEVE ser chamado em OnAfterRenderAsync(firstRender: true)
    /// </summary>
    public async Task<bool> InitializeAsync()
    {
        if (_isInitialized)
            return !string.IsNullOrEmpty(_tokenCache);

        _logger.LogInformation("🔄 Inicializando autenticação...");

        // 1. Tentar carregar do cookie HTTP primeiro (mais confiável)
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null && httpContext.Request.Cookies.TryGetValue(CookieName, out var cookieToken))
        {
            _tokenCache = cookieToken;
            _isInitialized = true;
            _logger.LogInformation("✅ Token recuperado do Cookie HTTP (tamanho: {Length})", _tokenCache.Length);
            return true;
        }

        // 2. Tentar carregar do ProtectedLocalStorage
        try
        {
            var result = await _localStorage.GetAsync<string>(TokenKey);
            
            if (result.Success && !string.IsNullOrEmpty(result.Value))
            {
                _tokenCache = result.Value;
                _isInitialized = true;
                _logger.LogInformation("✅ Token recuperado do ProtectedLocalStorage (tamanho: {Length})", _tokenCache.Length);
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Erro ao carregar token do storage (prerendering?)");
        }

        _logger.LogInformation("ℹ️ Nenhum token encontrado");
        _isInitialized = true;
        return false;
    }

    /// <summary>
    /// Salva o token (async - tenta cookie HTTP + storage)
    /// </summary>
    public async Task SetTokenAsync(string token)
    {
        _logger.LogInformation("💾 Salvando token (tamanho: {Length})", token.Length);
        
        // Sempre salva em memória primeiro
        _tokenCache = token;
        _isInitialized = true;

        // 1. Tentar salvar em cookie HTTP (se response ainda não foi enviada)
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null && !httpContext.Response.HasStarted)
        {
            try
            {
                httpContext.Response.Cookies.Append(CookieName, token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(8),
                    Path = "/"
                });
                _logger.LogInformation("✅ Token salvo em Cookie HTTP");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ Não foi possível salvar cookie (response já iniciada)");
            }
        }

        // 2. Sempre salvar em ProtectedLocalStorage como backup
        try
        {
            await _localStorage.SetAsync(TokenKey, token);
            _logger.LogInformation("✅ Token salvo em ProtectedLocalStorage");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Erro ao salvar em ProtectedLocalStorage (prerendering?)");
        }
    }

    /// <summary>
    /// Remove o token
    /// </summary>
    public async Task ClearTokenAsync()
    {
        _logger.LogInformation("🗑️ Limpando token");
        
        _tokenCache = null;
        _isInitialized = false;

        // 1. Remover cookie HTTP
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null && !httpContext.Response.HasStarted)
        {
            try
            {
                httpContext.Response.Cookies.Delete(CookieName);
                _logger.LogInformation("✅ Cookie HTTP removido");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ Erro ao remover cookie");
            }
        }

        // 2. Remover do ProtectedLocalStorage
        try
        {
            await _localStorage.DeleteAsync(TokenKey);
            _logger.LogInformation("✅ Token removido do ProtectedLocalStorage");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Erro ao remover do storage");
        }
    }

    public bool HasToken()
    {
        return !string.IsNullOrEmpty(_tokenCache);
    }
}
