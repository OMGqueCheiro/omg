using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;

namespace OMG.WebApp.Authentication;

/// <summary>
/// Serviço aprimorado de autenticação que persiste o token entre reloads
/// Usa ProtectedLocalStorage para armazenamento seguro no navegador (localStorage)
/// </summary>
public class PersistentAuthenticationStateService
{
    private readonly ProtectedLocalStorage _localStorage;
    private readonly ILogger<PersistentAuthenticationStateService> _logger;
    private const string TokenKey = "auth_token";
    private string? _token;
    private bool _isInitialized = false;
    private bool _storageAvailable = false;

    public PersistentAuthenticationStateService(
        ProtectedLocalStorage localStorage,
        ILogger<PersistentAuthenticationStateService> logger)
    {
        _localStorage = localStorage;
        _logger = logger;
    }

    /// <summary>
    /// Obtém o token. Tenta carregar do storage se ainda não inicializado e storage estiver disponível.
    /// Durante prerendering, retorna null.
    /// </summary>
    public async Task<string?> GetTokenAsync()
    {
        // Se já inicializou, retorna o token em memória
        if (_isInitialized)
        {
            return _token;
        }

        // Tenta carregar do storage (pode falhar durante prerendering)
        await TryInitializeFromStorageAsync();
        
        return _token;
    }

    public async Task SetTokenAsync(string token)
    {
        _logger.LogInformation("💾 Salvando token (tamanho: {Length})", token.Length);
        
        _token = token;
        _isInitialized = true;
        
        // Tenta salvar no storage (pode falhar durante prerendering, mas não é crítico)
        try
        {
            await _localStorage.SetAsync(TokenKey, token);
            _storageAvailable = true;
            _logger.LogInformation("✅ Token salvo no ProtectedLocalStorage");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("prerendering"))
        {
            _logger.LogWarning("⚠️ Storage não disponível durante prerendering - token em memória apenas");
            _storageAvailable = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao salvar token no storage");
            _storageAvailable = false;
        }
    }

    public async Task ClearTokenAsync()
    {
        _logger.LogInformation("🗑️ Limpando token");
        
        _token = null;
        _isInitialized = true;
        
        if (!_storageAvailable)
        {
            _logger.LogInformation("ℹ️ Storage não disponível - token já limpo da memória");
            return;
        }
        
        try
        {
            await _localStorage.DeleteAsync(TokenKey);
            _logger.LogInformation("✅ Token removido do ProtectedLocalStorage");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("prerendering"))
        {
            _logger.LogWarning("⚠️ Storage não disponível durante prerendering");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao remover token do storage");
        }
    }

    public async Task<bool> HasTokenAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    /// <summary>
    /// Tenta carregar token do storage. Se falhar (prerendering), não é um erro crítico.
    /// </summary>
    private async Task TryInitializeFromStorageAsync()
    {
        if (_isInitialized)
            return;

        try
        {
            var result = await _localStorage.GetAsync<string>(TokenKey);
            
            if (result.Success && !string.IsNullOrEmpty(result.Value))
            {
                _token = result.Value;
                _storageAvailable = true;
                _logger.LogInformation("✅ Token recuperado do ProtectedLocalStorage (tamanho: {Length})", _token.Length);
            }
            else
            {
                _logger.LogInformation("ℹ️ Nenhum token encontrado no storage");
            }
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("prerendering") || ex.Message.Contains("statically rendered"))
        {
            // Durante prerendering, isso é esperado - não é um erro
            _logger.LogDebug("🔄 Storage não disponível durante prerendering - aguardando render interativo");
            _storageAvailable = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao carregar token do storage");
            _storageAvailable = false;
        }
        finally
        {
            _isInitialized = true;
        }
    }
}
