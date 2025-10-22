using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;

namespace OMG.WebApp.Authentication;

/// <summary>
/// Serviço de autenticação que usa uma combinação de memória e ProtectedLocalStorage
/// com inicialização explícita via OnAfterRenderAsync
/// </summary>
public class CookieAuthenticationStateService
{
    private readonly ProtectedLocalStorage _localStorage;
    private readonly ILogger<CookieAuthenticationStateService> _logger;
    private const string TokenKey = "auth_token";
    private string? _token;
    private bool _isInitialized = false;

    public CookieAuthenticationStateService(
        ProtectedLocalStorage localStorage,
        ILogger<CookieAuthenticationStateService> logger)
    {
        _localStorage = localStorage;
        _logger = logger;
    }

    /// <summary>
    /// Retorna o token em memória. Não tenta carregar do storage.
    /// Use InitializeAsync() explicitamente em OnAfterRenderAsync.
    /// </summary>
    public string? GetToken()
    {
        return _token;
    }

    /// <summary>
    /// Inicializa carregando token do storage.
    /// DEVE ser chamado de OnAfterRenderAsync(firstRender) para garantir que JS Interop está disponível.
    /// </summary>
    public async Task<bool> InitializeAsync()
    {
        if (_isInitialized)
            return !string.IsNullOrEmpty(_token);

        _logger.LogInformation("🔄 Inicializando autenticação do storage...");

        try
        {
            var result = await _localStorage.GetAsync<string>(TokenKey);
            
            if (result.Success && !string.IsNullOrEmpty(result.Value))
            {
                _token = result.Value;
                _logger.LogInformation("✅ Token recuperado do ProtectedLocalStorage (tamanho: {Length})", _token.Length);
                _isInitialized = true;
                return true;
            }
            else
            {
                _logger.LogInformation("ℹ️ Nenhum token encontrado no storage");
                _isInitialized = true;
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao carregar token do storage");
            _isInitialized = true;
            return false;
        }
    }

    public async Task SetTokenAsync(string token)
    {
        _logger.LogInformation("💾 Salvando token (tamanho: {Length})", token.Length);
        
        _token = token;
        _isInitialized = true;
        
        try
        {
            await _localStorage.SetAsync(TokenKey, token);
            _logger.LogInformation("✅ Token salvo no ProtectedLocalStorage");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao salvar token no storage");
        }
    }

    public async Task ClearTokenAsync()
    {
        _logger.LogInformation("🗑️ Limpando token");
        
        _token = null;
        _isInitialized = false;
        
        try
        {
            await _localStorage.DeleteAsync(TokenKey);
            _logger.LogInformation("✅ Token removido do ProtectedLocalStorage");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao remover token do storage");
        }
    }

    public bool HasToken()
    {
        return !string.IsNullOrEmpty(_token);
    }
}
